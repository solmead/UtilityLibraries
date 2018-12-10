using PostSharp.Aspects;
using PostSharp.Serialization;
using Utilities.Caching.Aspects.Naming;

namespace Utilities.Caching.Aspects
{
    [PSerializable]
    public class CacheProperty : LocationInterceptionAspect
    {
        public CacheArea CacheArea { get; set; }
        public string BaseName { get; set; }
        public double LifeSpanSeconds { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public ICacheEntryNamer Namer { get; set; }

        public CacheProperty()
        {
            //ApplyToStateMachine = false;
        }


        public ICacheEntryNamer GetNamer()
        {
            if (Namer == null)
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    Namer = new StringNamer(Name);
                }
                if (Namer == null)
                {
                    Namer = new BaseNamer();
                }
            }
            return Namer;
        }

        public override void OnGetValue(LocationInterceptionArgs args)
        {

            var mthInfo = args.Location;
            var name = GetNamer().GetName(BaseName, args);
            var retType = mthInfo.LocationType;

            var ret = Cache.GetItem(CacheArea, name, retType, null, Tags);
            if (ret != null)
            {
                args.Value = ret;
            }
            else
            {
                base.OnGetValue(args);
                Cache.SetItem(CacheArea, name, retType, args.Value, Tags);
            }

        }

        public override void OnSetValue(LocationInterceptionArgs args)
        {
            base.OnSetValue(args);
            var mthInfo = args.Location;
            var retType = mthInfo.LocationType;

            var name = GetNamer().GetName(BaseName, args);
            Cache.SetItem(CacheArea, name, retType, args.Value, Tags);

        }
    }
}
