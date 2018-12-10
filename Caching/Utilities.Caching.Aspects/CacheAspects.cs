using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using Utilities.Caching.Aspects;
using Utilities.Caching.Aspects.Helpers;

namespace Utilities.Caching
{
    public class CacheAspects
    {


        public static void SetOnFunction<t1, t2, t3, t4, tResult>(Func<t1, t2, t3, t4, tResult> method, List<object> args, tResult obj)
        {
            SetOnFunction(method.Method, args, obj);
        }
        public static void SetOnFunction<t1, t2, t3, tResult>(Func<t1, t2, t3, tResult> method, List<object> args, tResult obj)
        {
            SetOnFunction(method.Method, args, obj);
        }
        public static void SetOnFunction<t1, t2, tResult>(Func<t1, t2, tResult> method, List<object> args, tResult obj)
        {
            SetOnFunction(method.Method, args, obj);
        }
        public static void SetOnFunction<t1, tResult>(Func<t1, tResult> method, List<object> args, tResult obj)
        {
            SetOnFunction(method.Method, args, obj);
        }
        public static void SetOnFunction<tResult>(Func<tResult> method, tResult obj)
        {
            SetOnFunction(method.Method, null, obj);
        }
        public static void SetOnProperty<tResult>(Expression<Func<tResult>> property, tResult obj)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;

            SetOnProperty(propertyInfo, obj);
        }

        public static void SetOnProperty<tt>(PropertyInfo method, tt obj)
        {
            var mi = method;
            var caList = GetAttributesOfProperty(mi);

            caList.ForEach((ca) =>
            {
                Cache.SetItem<object>(ca.CacheArea, ca.GetNamer().GetName(ca.BaseName, mi), obj, ca.LifeSpanSeconds);
            });
        }

        public static void SetOnFunction<tt>(MethodInfo method, List<object> args, tt obj)
        {
            var mi = method;
            var caList = GetAttributesOfFunction(mi);
            var param = mi.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                object v = null;
                if (args!=null && args.Count > p.Position)
                {
                    v = args[p.Position];
                }
                else if (p.HasDefaultValue)
                {
                    v = p.DefaultValue;
                }
                dic.Add(p.Name, v);
            }
            caList.ForEach((ca) =>
            {
                Cache.SetItem<object>(ca.CacheArea, ca.GetNamer().GetName(ca.BaseName, mi, dic), obj, ca.LifeSpanSeconds);
            });
        }


        public static async Task SetOnFunctionAsync<t1, t2, t3, t4, tResult>(Func<t1, t2, t3, t4, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetOnFunctionAsync(method.Method, args, obj);
        }
        public static async Task SetOnFunctionAsync<t1, t2, t3, tResult>(Func<t1, t2, t3, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetOnFunctionAsync(method.Method, args, obj);
        }
        public static async Task SetOnFunctionAsync<t1, t2, tResult>(Func<t1, t2, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetOnFunctionAsync(method.Method, args, obj);
        }
        public static async Task SetOnFunctionAsync<t1, tResult>(Func<t1, Task<tResult>> method, List<object> args, tResult obj)
        {
            await SetOnFunctionAsync(method.Method, args, obj);
        }
        public static async Task SetOnFunctionAsync<tResult>(Func<Task<tResult>> method, tResult obj)
        {
            await SetOnFunctionAsync(method.Method, null, obj);
        }
       

        public static async Task SetOnFunctionAsync<tt>(MethodInfo method, List<object> args, tt obj)
        {
            var mi = method;
            var caList = GetAttributesOfFunction(mi);
            var param = mi.GetParameters();
            var dic = new Dictionary<string, object>();
            foreach (var p in param)
            {
                object v = null;
                if (args != null && args.Count > p.Position)
                {
                    v = args[p.Position];
                }
                else if (p.HasDefaultValue)
                {
                    v = p.DefaultValue;
                }
                dic.Add(p.Name, v);
            }
            await caList.ForEachAsync(async(ca) =>
            {
                await Cache.SetItemAsync<object>(ca.CacheArea, ca.GetNamer().GetName(ca.BaseName, mi, dic), obj, ca.LifeSpanSeconds);
            });
            
        }



        private static List<CacheFunction> GetAttributesOfFunction(MethodInfo method)
        {
            var pInfo = method
                             .GetCustomAttributes(typeof(CacheFunction), false)
                             .Cast<CacheFunction>().ToList();

            return pInfo;
        }
        private static List<CacheProperty> GetAttributesOfProperty(PropertyInfo method)
        {
            var pInfo = method
                             .GetCustomAttributes(typeof(CacheProperty), false)
                             .Cast<CacheProperty>().ToList();

            return pInfo;
        }
    }
}
