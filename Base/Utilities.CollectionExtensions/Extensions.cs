using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Extensions
{
    public static class Extensions
    {
        public static IAsyncList<Tt> AsAsyncList<Tt>(this List<Tt> baselst)
        {
            return new AsyncList<Tt>(baselst);
        }
        public static IAsyncList<Tt> AsAsyncList<Tt>(this IEnumerable<Tt> baselst)
        {
            return new AsyncList<Tt>(baselst.ToList());
        }
        public static IAsyncList<Tt> AsAsyncList<Tt>(this IQueryable<Tt> baselst)
        {
            return new AsyncList<Tt>(baselst.ToList());
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
    }
}
