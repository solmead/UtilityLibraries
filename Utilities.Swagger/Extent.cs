using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Swagger
{
    public static class Extent
    {
        public static string ToCamelCasing(this string str)
        {

            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
            //return helper.Replace(helper[0].ToString(), helper[0].ToString().ToLower());
        }


        public static string ReplaceTypeName(this string type, string from, string to)
        {

            if (type.ToUpper()==from.ToUpper())
            {
                return to;
            }

            if (type.Contains(from))
            {
                var i = 1;
            }


            return type;

        }
    }
}
