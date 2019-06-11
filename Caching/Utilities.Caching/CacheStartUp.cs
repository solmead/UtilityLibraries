using System;
using System.Collections.Generic;
using System.Text;
using Utilities.SerializeExtensions.Serializers;

namespace Utilities.Caching
{
    public static class CacheStartUp
    {

        public static ISerializer Serializer
        {
            get => CacheSystem.Serializer;
            set => CacheSystem.Serializer = value;
        }
        public static void LogDebug(string msg)
        {
            CacheSystem.Instance.LogDebugMessage?.Invoke(msg);
        }
        public static void LogError(string msg)
        {
            CacheSystem.Instance.LogErrorMessage?.Invoke(msg);
        }

        public static void Init()
        {
            var system = CacheSystem.Instance;
        }
    }
}
