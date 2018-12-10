using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities.Caching;
using Utilities.Caching.Core.Extras;
using Utilities.Caching.Database.Models;

namespace Utilities.Caching.Database.Context
{
    public class StoreInDatabase : IPermanentRepository
    {

        public DbContextOptions<DataCacheContext> ContextOptions { get; set; }
        private string _connectionString { get; set; }

        public StoreInDatabase(string connectionString)
        {
            _connectionString = connectionString;

            var d = new DbContextOptionsBuilder(new DbContextOptions<DataCacheContext>());
            var e = d.UseSqlServer(connectionString);
            ContextOptions = e.Options as DbContextOptions<DataCacheContext>;

            //options.UseSqlServer(connection)
            using (var database = new DataCacheContext(ContextOptions))
            {
                database.UpgradeDB();
            }
        }


        private static Dictionary<string, CachedEntry> ValuesDictionary
        {
            get => Cache.GetItem<Dictionary<string, CachedEntry>>(CacheArea.Global, "StoreInDatabase_ValuesDictionary",
                () => new Dictionary<string, CachedEntry>());
            set => Cache.SetItem<Dictionary<string, CachedEntry>>(CacheArea.Global, "StoreInDatabase_ValuesDictionary", value);
        }

        private void CleanOutTimeOutValues(DataCacheContext database)
        {
                try
                {

                    var lst = (from ce in database.CachedEntries
                         where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                         select ce).ToList();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                        database.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                }
        }
        private async Task CleanOutTimeOutValuesAsync(DataCacheContext database)
        {
                try
                {
                    var lst = await
                        (from ce in database.CachedEntries
                         where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                         select ce).ToListAsync();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                        await database.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {

                }
        }

        private async Task<CachedEntry> GetItemAsync(string name)
        {
            CachedEntry itm = null;


            if (ValuesDictionary.ContainsKey(name.ToUpper()))
            {
                itm = ValuesDictionary[name.ToUpper()];
            }

            //lock (_lock)
            if (itm == null)
            {
                using (var database = new DataCacheContext(ContextOptions))
                {
                    itm = await (from ce in database.CachedEntries.AsNoTracking() where ce.Name == name select ce).FirstOrDefaultAsync();
                }
            }

            itm = itm ?? new CachedEntry()
            {
                Created = DateTime.Now,
                Name = name,
                Object = ""
            };

            if (ValuesDictionary.ContainsKey(name.ToUpper()))
            {
                ValuesDictionary[name.ToUpper()] = itm;
            }
            else
            {
                ValuesDictionary.Add(name.ToUpper(), itm);
            }

            return itm;
        }

        private  CachedEntry GetItem(string name)
        {
            CachedEntry itm = null;


            if (ValuesDictionary.ContainsKey(name.ToUpper()))
            {
                itm = ValuesDictionary[name.ToUpper()];
            }

            //lock (_lock)
            if (itm == null)
            {
                using (var database = new DataCacheContext(ContextOptions))
                {
                    itm =  (from ce in database.CachedEntries.AsNoTracking() where ce.Name == name select ce).FirstOrDefault();
                }
            }

            itm = itm ?? new CachedEntry()
            {
                Created = DateTime.Now,
                Name = name,
                Object = ""
            };

            if (ValuesDictionary.ContainsKey(name.ToUpper()))
            {
                ValuesDictionary[name.ToUpper()] = itm;
            }
            else
            {
                ValuesDictionary.Add(name.ToUpper(), itm);
            }

            return itm;
        }



        public async Task<byte[]> GetAsync(string name)
        {
            var itm = await GetItemAsync(name);
            if (itm != null && itm.TimeOut.HasValue && itm.TimeOut.Value >= DateTime.Now)
            {
                var xml = itm.Object;
                try
                {
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var o = Convert.FromBase64String(xml);
                        return o;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            
            return new byte[0];
        }

        public async Task SetAsync(string name, byte[] value, TimeSpan? timeout)
        {

            var xml = Convert.ToBase64String(value);

            var itm = await GetItemAsync(name);

            using (var database = new DataCacheContext(ContextOptions))
            {
                if (itm.Id > 0)
                {
                    database.CachedEntries.Attach(itm);
                }
                else
                {
                    database.CachedEntries.Add(itm);
                }

                if (value.Length == 0)
                {
                    if (itm.Id!=0)
                    {
                        database.CachedEntries.Remove(itm);
                    }

                    ValuesDictionary[name.ToUpper()] = null;
                }
                else
                {
                    DateTime? endTime = null;
                    if (timeout.HasValue)
                    {
                        endTime = DateTime.Now.AddSeconds(timeout.Value.TotalSeconds);
                    }
                    itm.TimeOut = endTime;
                    itm.Changed = DateTime.Now;
                    itm.Object = xml;
                }


                await database.SaveChangesAsync();
                database.Entry(itm).State = EntityState.Detached;
                //await CleanOutTimeOutValuesAsync(database);
            }

        }

        public async Task DeleteAsync(string name)
        {
            using (var database = new DataCacheContext(ContextOptions))
            {
                var lst = await(from ce in database.CachedEntries where ce.Name == name select ce).ToListAsync();
                if (lst.Count > 0)
                {
                    database.CachedEntries.RemoveRange(lst);
                }
                await database.SaveChangesAsync();
            }
        }

        public Task<List<string>> GetKeysAsync()
        {
            throw new NotImplementedException();
        }

        public byte[] Get(string name)
        {


            var itm =  GetItem(name);
            if (itm != null && itm.TimeOut.HasValue && itm.TimeOut.Value >= DateTime.Now)
            {
                var xml = itm.Object;
                try
                {
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var o = Convert.FromBase64String(xml);
                        return o;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            return new byte[0];
        }

        public void Set(string name, byte[] value, TimeSpan? timeout)
        {
            var xml = Convert.ToBase64String(value);

            var itm =  GetItem(name);

            using (var database = new DataCacheContext(ContextOptions))
            {
                if (value.Length == 0)
                {
                    ValuesDictionary[name.ToUpper()] = null;
                    if (itm.Id != 0)
                    {
                        database.CachedEntries.Remove(itm);
                        database.SaveChanges();
                    }
                }
                else if (value.Length>0)
                {
                    DateTime? endTime = null;
                    if (timeout.HasValue)
                    {
                        endTime = DateTime.Now.AddSeconds(timeout.Value.TotalSeconds);
                    }
                    itm.TimeOut = endTime;
                    itm.Changed = DateTime.Now;
                    itm.Object = xml;

                    if (itm.Id > 0)
                    {
                        database.CachedEntries.Attach(itm);
                    }
                    else
                    {
                        database.CachedEntries.Add(itm);
                    }
                    database.SaveChanges();
                    database.Entry(itm).State = EntityState.Detached;
                }



                //await CleanOutTimeOutValuesAsync(database);
            }

        }

        public void Delete(string name)
        {
            using (var database = new DataCacheContext(ContextOptions))
            {
                var lst = (from ce in database.CachedEntries where ce.Name == name select ce).ToList();
                if (lst.Count > 0)
                {
                    database.CachedEntries.RemoveRange(lst);
                }
                database.SaveChanges();
            }
        }

        public List<string> GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
