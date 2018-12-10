using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Utilities.Caching;
using Utilities.Caching.Database.Models;

namespace Utilities.Caching.Database.Context
{
    public class DataCacheContext : DbContext
    {

        public DataCacheContext(DbContextOptions<DataCacheContext> options)
            : base(options)
        { }


        public void UpgradeDB()
        {
            Database.Migrate();
            //Database.SetInitializer<DataContext>(null);
            //try
            //{
            //    var configuration = new Configuration();
            //    configuration.TargetDatabase = new DbConnectionInfo(Settings.Default.ConnectionName);
            //    var dbMigrator = new DbMigrator(configuration);

            //    if (dbMigrator.GetPendingMigrations().Count() > 0)
            //    {
            //        dbMigrator.Update();
            //        dbMigrator.Update();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.ToString());
            //    throw ex;
            //}
        }

        
        //public static void ClearContext()
        //{
        //    DataContext context = null;
        //    Cache.SetItem(CacheArea.Request, "DbCachingDataContext", context);
        //}
        //public static DataContext Current
        //{
        //    get
        //    {

        //        return Cache.GetItem<DataContext>(CacheArea.Request, "DbCachingDataContext", () => new DataContext());
        //    }
        //}

        public DbSet<CachedEntry> CachedEntries { get; set; }
    }
}
