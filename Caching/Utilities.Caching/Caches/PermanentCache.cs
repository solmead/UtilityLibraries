using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Caching.Core;
using Utilities.Caching.Core.DataSources;
using Utilities.Caching.Core.Extras;

namespace Utilities.Caching.Caches
{
    public class PermanentCache : DataCache
    {
        private IPermanentRepository _permanentRepo = null;

        public IPermanentRepository Repository
        {
            get
            {
                var cDB = baseDataSource.CacheRepo;
                if (cDB == null)
                {
                    cDB = _permanentRepo;
                    
                    baseDataSource.CacheRepo = cDB;
                }

                return baseDataSource.CacheRepo;
            }
            set { baseDataSource.CacheRepo = value; }
        }
        private PermanentDataSource baseDataSource { get; set; }


        public PermanentCache(IPermanentRepository permanentRepo)
            : base(new PermanentDataSource(permanentRepo))
        {
            Area = CacheArea.Permanent;
            Name = "BasePermanent";
            _permanentRepo = permanentRepo;
            baseDataSource = base.DataSource as PermanentDataSource;
            var repo = Repository;

        }



    }
}
