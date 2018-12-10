using Utilities.Caching.Core;
using Utilities.Caching.Core.DataSources;
using Utilities.Caching.Database.Context;

namespace Utilities.Caching.Database
{
    public class DatabaseCache : DataCache
    {

        public DatabaseCache(string connectionString)
            : base(new PermanentDataSource(new StoreInDatabase(connectionString)))
        {
            
        }

        public override CacheArea Area => CacheArea.Permanent;
        public override string Name => "DatabaseCache";
    }
}
