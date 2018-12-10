using System;
using System.Collections.Generic;
using System.Reflection;
using PostSharp.Aspects;

namespace Utilities.Caching.Aspects.Naming
{
    [Serializable]
    public class StringNamer : ICacheEntryNamer
    {
        public StringNamer(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public string GetName(string baseName, MethodInterceptionArgs args)
        {
            return Name;
        }

        public string GetName(string baseName, LocationInterceptionArgs args)
        {
            return Name;
        }

        public string GetName(string baseName, PropertyInfo method)
        {
            return Name;
        }
        public string GetName(string baseName, MethodInfo method, Dictionary<string, object> parameters = null)
        {
            return Name;
            
        }
        public string GetName(string baseName, MethodExecutionArgs args)
        {
            return Name;
        }
    }
}
