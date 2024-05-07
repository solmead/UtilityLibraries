using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger
{
    public static class Extent
    {
        public static string ToCamelCasing(this string helper)
        {
            return helper.Replace(helper[0].ToString(), helper[0].ToString().ToLower());
        }
    }
}
