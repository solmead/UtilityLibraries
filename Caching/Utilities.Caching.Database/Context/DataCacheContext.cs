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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }

        public void UpgradeDB()
        {
            Database.Migrate();
        }


        public DbSet<CachedEntry> CachedEntries { get; set; }
    }
}
