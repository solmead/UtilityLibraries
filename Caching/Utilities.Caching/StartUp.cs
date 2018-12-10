using System;
using System.Collections.Generic;
using System.Text;
using Utilities.SerializeExtensions.Serializers;

namespace Utilities.Caching
{
    public static class StartUp
    {

        public static ISerializer Serializer
        {
            get => Cache.GetItem<ISerializer>(CacheArea.Global, "CachingSerializer", () => new JsonSerializer());
            set => Cache.SetItem<ISerializer>(CacheArea.Global, "CachingSerializer", value);
        }

        public static void Init()
        {
            var system = CacheSystem.Instance;
        }
    }
}
