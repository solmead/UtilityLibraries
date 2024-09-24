using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Extensions
{
    public static class CollectionExt
    {
        public static IAsynchronousList<Tt> AsAsynchronousList<Tt>(this List<Tt> baselst)
        {
            return new AsynchronousList<Tt>(baselst);
        }
        public static IAsynchronousList<Tt> AsAsynchronousList<Tt>(this IEnumerable<Tt> baselst)
        {
            return new AsynchronousList<Tt>(baselst.ToList());
        }
        public static IAsynchronousList<Tt> AsAsynchronousList<Tt>(this IQueryable<Tt> baselst)
        {
            return new AsynchronousList<Tt>(baselst.ToList());
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            var rnd = new Random();
            var values =
                (from i in (from p in source.ToList() let z = rnd.Next(1, 100000) select new { prod = p, Rand = z })
                    orderby i.Rand
                    select i.prod);

            return values.ToList();
        }
        



        public static async Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> target, Func<T, Task<bool>> predicateAsync)
        {
            var tasks = target.Select(async x => new { Predicate = await predicateAsync(x), Value = x }).ToArray();
            var results = await Task.WhenAll(tasks);

            return results.Where(x => x.Predicate).Select(x => x.Value);
        }
        public static async Task<List<T>> WhereAsync<T>(this List<T> target, Func<T, Task<bool>> predicateAsync)
        {
            var tasks = target.Select(async x => new { Predicate = await predicateAsync(x), Value = x }).ToArray();
            var results = await Task.WhenAll(tasks);

            return results.Where(x => x.Predicate).Select(x => x.Value).ToList();
        }


        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> values, Func<TSource, Task<TResult>> asyncSelector)
        {
            return await Task.WhenAll(values.Select(asyncSelector).ToList());
        }
        public static async Task<List<TResult>> SelectAsync<TSource, TResult>(this List<TSource> values, Func<TSource, Task<TResult>> asyncSelector)
        {
            return (await Task.WhenAll(values.Select(asyncSelector).ToList())).ToList();
        }


        public static async Task ForEachAsync<T>(this IEnumerable<T> values, Func<T, Task> asyncAction)
        {
            foreach (var i in values)
            {
                await asyncAction(i);
            }
        }
        public static async Task ForEachAsync<T>(this List<T> values, Func<T, Task> asyncAction)
        {
            foreach (var i in values)
            {
                await asyncAction(i);
            }
        }

        public static Task WhenAllAsync<T>(this IEnumerable<T> values, Func<T, Task> asyncAction)
        {
            return Task.WhenAll(values.Select(asyncAction).ToList());
        }
        public static Task WhenAllAsync<T>(this List<T> values, Func<T, Task> asyncAction)
        {
            return WhenAllAsync((IEnumerable<T>)values, asyncAction);
        }
    }
}
