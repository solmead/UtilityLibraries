using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Validators
{
    internal static class Ext
    {
        public static bool LessThan(this object obj1, object obj2)
        {

            var type = obj1.GetType();

            var tName = type.FullName.ToUpper();
            if ((tName.Contains("DATETIME")))
            {
                return ((DateTime)obj1) < ((DateTime)obj2);
            }
            else if ((tName.Contains("BOOL")))
            {
                return ((bool)obj1) != ((bool)obj2);
            }
            else if ((tName.Contains("INT")))
            {
                return ((int)obj1) < ((int)obj2);
            }
            else if ((tName.Contains("FLOAT")))
            {
                return ((float)obj1) < ((float)obj2);
            }
            else if ((tName.Contains("DOUBLE")))
            {
                return ((double)obj1) < ((double)obj2);
            }
            else if ((tName.Contains("LONG")))
            {
                return ((long)obj1) < ((long)obj2);
            }
            else if ((tName.Contains("DECIMAL")))
            {
                return ((decimal)obj1) < ((decimal)obj2);
            }
            else
            {
                return (obj1.ToString().CompareTo(obj2.ToString()) <0);
            }
        }
        public static object FromString(this Type type, string value)
        {
            var tName = type.FullName.ToUpper();
            if ((tName.Contains("DATETIME")))
            {
                System.DateTime v = default(System.DateTime);
                DateTime.TryParse(value, out v);
                return v;
            }
            else if ((tName.Contains("BOOL")))
            {
                if (value == null)
                {
                    value = "";
                }
                value = value.ToString().ToUpper().Replace("YES", "TRUE").Replace("NO", "FALSE").Replace("0", "FALSE").Replace("N", "FALSE").Replace("Y", "TRUE").Replace("1", "TRUE");
                bool v = false;
                bool.TryParse(value.ToString(), out v);
                return v;
            }
            else if ((tName.Contains("INT")))
            {
                if (value == null)
                {
                    value = "";
                }
                value = value.ToString().Replace("$", "").Replace(",", "");
                int v = 0;
                int.TryParse(value.ToString(), out v);
                return v;
            }
            else if ((tName.Contains("FLOAT")))
            {
                if (value == null)
                {
                    value = "";
                }
                value = value.ToString().Replace("$", "").Replace(",", "");
                float v = 0;
                float.TryParse(value.ToString(), out v);
                return v;
            }
            else if ((tName.Contains("DOUBLE")))
            {
                if (value == null)
                {
                    value = "";
                }
                value = value.ToString().Replace("$", "").Replace(",", "");
                double v = 0;
                double.TryParse(value.ToString(), out v);
                return v;
            }
            else if ((tName.Contains("LONG")))
            {
                if (value == null)
                {
                    value = "";
                }
                value = value.ToString().Replace("$", "").Replace(",", "");
                long v = 0;
                long.TryParse(value.ToString(), out v);
                return v;
            }
            else if ((tName.Contains("DECIMAL")))
            {
                if (value == null)
                {
                    value = "";
                }
                value = value.ToString().Replace("$", "").Replace(",", "");
                decimal v = default(decimal);
                decimal.TryParse(value.ToString(), out v);
                return v;
            }
            else
            {
                return value;
            }
        }
    }
}
