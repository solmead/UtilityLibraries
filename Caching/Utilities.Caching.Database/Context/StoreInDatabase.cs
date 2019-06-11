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

        private async Task<CachedEntry> GetItemAsync(string name, bool useDic = true)
        {
            CachedEntry itm = null;


            if (useDic &&  ValuesDictionary.ContainsKey(name.ToUpper()))
            {
                itm = ValuesDictionary[name.ToUpper()];
            }

            //lock (_lock)
            if (itm == null)
            {
                using (var database = new DataCacheContext(ContextOptions))
                {
                    itm = await (from ce in database.CachedEntries
                                 where ce.Name == name.ToUpper()
                                 select new CachedEntry()
                                 {
                                     Id = ce.Id,
                                     Changed = ce.Changed,
                                     Created = ce.Created,
                                     Name = ce.Name,
                                     Object = ce.Object,
                                     TimeOut = ce.TimeOut
                                 }).FirstOrDefaultAsync();
                }
            }

            //itm = itm ?? new CachedEntry()
            //{
            //    Created = DateTime.Now,
            //    Name = name,
            //    Object = ""
            //};

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

        private  CachedEntry GetItem(string name, bool useDic = true)
        {
            CachedEntry itm = null;


            if (useDic && ValuesDictionary.ContainsKey(name.ToUpper()))
            {
                itm = ValuesDictionary[name.ToUpper()];
            }

            //lock (_lock)
            if (itm == null)
            {
                using (var database = new DataCacheContext(ContextOptions))
                {
                    itm =  (from ce in database.CachedEntries
                            where ce.Name == name.ToUpper()
                            select new CachedEntry()
                            {
                                Id = ce.Id,
                                Changed = ce.Changed,
                                Created = ce.Created,
                                Name = ce.Name,
                                Object = ce.Object,
                                TimeOut = ce.TimeOut
                            }).FirstOrDefault();
                }
            }

            //itm = itm ?? new CachedEntry()
            //{
            //    Created = DateTime.Now,
            //    Name = name,
            //    Object = ""
            //};

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

            //var itm = await GetItemAsync(name);
            //itm = itm ?? new CachedEntry()
            //{
            //    Created = DateTime.Now,
            //    Name = name,
            //    Object = ""
            //};

            using (var database = new DataCacheContext(ContextOptions))
            {

                var itm = await (from ce in database.CachedEntries
                           where ce.Name == name.ToUpper()
                                 select ce).FirstOrDefaultAsync();
                if (itm == null)
                {
                    itm = new CachedEntry()
                    {
                        Created = DateTime.Now,
                        Name = name.ToUpper(),
                        Object = ""
                    };
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
                //database.Entry(itm).State = EntityState.Detached;
                //await CleanOutTimeOutValuesAsync(database);

                itm = await GetItemAsync(name, false);
            }

        }

        public async Task DeleteAsync(string name)
        {
            using (var database = new DataCacheContext(ContextOptions))
            {
                var lst = await(from ce in database.CachedEntries where ce.Name == name.ToUpper() select ce).ToListAsync();
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
                Debug.WriteLine("Data from DB:" + xml);
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
            Debug.WriteLine("Data to DB:" + xml);
            //var itm =  GetItem(name);

            //itm = itm ?? new CachedEntry()
            //{
            //    Created = DateTime.Now,
            //    Name = name,
            //    Object = ""
            //};

            using (var database = new DataCacheContext(ContextOptions))
            {

                var itm = (from ce in database.CachedEntries
                                where ce.Name == name.ToUpper()
                           select ce).FirstOrDefault();
                if (itm == null)
                {
                    itm = new CachedEntry()
                    {
                        Created = DateTime.Now,
                        Name = name.ToUpper(),
                        Object = ""
                    };
                    database.CachedEntries.Add(itm);
                }


                if (value.Length == 0)
                {
                    if (itm.Id != 0)
                    {
                        database.CachedEntries.Remove(itm);
                    }
                    ValuesDictionary[name.ToUpper()] = null;
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

                }
                database.SaveChanges();

                itm = GetItem(name, false);
                itm = itm;

                //await CleanOutTimeOutValuesAsync(database);
            }

        }

        public void Delete(string name)
        {
            using (var database = new DataCacheContext(ContextOptions))
            {
                var lst = (from ce in database.CachedEntries where ce.Name == name.ToUpper() select ce).ToList();
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
