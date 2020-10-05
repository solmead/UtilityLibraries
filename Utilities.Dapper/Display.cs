using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace Utilities.Dapper
{
    public static class Display
    {

        public static string ArgsAsSql(string sql, List<DbParameter> parameters)
        {
            var sb = new StringBuilder();
            sql = sql.Split('@')[0];
            sb.Append(sql);
            sb.Append(" ");
            var first = true;
            foreach (var param in parameters)
            {
                object pValue = null;
                var name = "";

                //var p = param as ObjectParameter;

                //if (p != null)
                //{
                //    pValue = p.Value;
                //    name = p.Name;
                //}
                var p2 = param as DbParameter;
                if (p2 != null)
                {
                    pValue = p2.Value;
                    name = p2.ParameterName;
                }

                name = name.Replace("@", "");

                var type = pValue?.GetType() ?? typeof(string);
                if (!first)
                {
                    sb.Append(", ");
                }
                if (pValue == null || pValue == DBNull.Value)
                    sb.AppendFormat("@{0} = NULL", name);
                else if (type == typeof(DateTime))
                    sb.AppendFormat("@{0} ='{1}'", name, ((DateTime)pValue).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                else if (type == typeof(bool))
                    sb.AppendFormat("@{0} = {1}", name, (bool)pValue ? 1 : 0);
                else if (type == typeof(int))
                    sb.AppendFormat("@{0} = {1}", name, pValue);
                else if (type == typeof(long))
                    sb.AppendFormat("@{0} = {1}", name, pValue);
                else if (type == typeof(float))
                    sb.AppendFormat("@{0} = {1}", name, pValue);
                else if (type == typeof(double))
                    sb.AppendFormat("@{0} = {1}", name, pValue);
                else
                    sb.AppendFormat("@{0} = '{1}'", name, pValue?.ToString());

                first = false;

            }


            return sb.ToString();
        }

        //public static string ArgsAsSql(this DbContext db, string sql, List<DbParameter> parameters)
        //{
        //    return ArgsAsSql(sql, parameters);
        //}

        public static string ArgsAsSql(this IDbConnection db, string sql, List<DbParameter> parameters)
        {
            return ArgsAsSql(sql, parameters);
        }

        //public static void DebugWrite(this IDbConnection db, string sql, List<DbParameter> parameters)
        //{
        //    var st = ArgsAsSql(sql, parameters);
        //    SqlUtilities.Log(st);
        //}
        ////public static void DebugWrite(this DbContext db, string sql, List<DbParameter> parameters)
        ////{
        ////    var st = ArgsAsSql(sql, parameters);
        ////    SqlUtilities.Log(st);
        ////}
        ////public static void DebugWrite(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        ////{
        ////    var st = ArgsAsSql(sql, parameters);
        ////    SqlUtilities.Log(st);
        ////}
        //public static void DebugWrite(string sql, List<DbParameter> parameters)
        //{
        //    var st = ArgsAsSql(sql, parameters);
        //    SqlUtilities.Log(st);
        //}


    }
}
