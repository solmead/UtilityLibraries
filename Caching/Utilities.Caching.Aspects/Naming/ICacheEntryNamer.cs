using System.Collections.Generic;
using System.Reflection;
using PostSharp.Aspects;

namespace Utilities.Caching.Aspects.Naming
{
   public interface ICacheEntryNamer
   {
        //MethodInterceptionArgs
        string GetName(string baseName, MethodExecutionArgs args);
        string GetName(string baseName, MethodInterceptionArgs args);
        string GetName(string baseName, LocationInterceptionArgs args);
        string GetName(string baseName, MethodInfo method, Dictionary<string, object> parameters = null);
        string GetName(string baseName, PropertyInfo method);
    }
}
