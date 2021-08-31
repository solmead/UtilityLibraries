using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Utilities.Poco;

namespace Utilities.Sql
{
    public static class SqlUtilities
    {

        public static Action<string> LogMessage { get; set; }

        private static Dictionary<Type, DbType> _typeMap;

        public static void Log(string msg)
        {
            //Debug.WriteLine(msg);
            LogMessage?.Invoke(msg);
        }

        private static Dictionary<Type, DbType> TypeMap
        {
            get
            {
                if (_typeMap == null)
                {
                    var typeMap = new Dictionary<Type, DbType>();
                    typeMap[typeof(byte)] = DbType.Byte;
                    typeMap[typeof(sbyte)] = DbType.SByte;
                    typeMap[typeof(short)] = DbType.Int16;
                    typeMap[typeof(ushort)] = DbType.UInt16;
                    typeMap[typeof(int)] = DbType.Int32;
                    typeMap[typeof(uint)] = DbType.UInt32;
                    typeMap[typeof(long)] = DbType.Int64;
                    typeMap[typeof(ulong)] = DbType.UInt64;
                    typeMap[typeof(float)] = DbType.Single;
                    typeMap[typeof(double)] = DbType.Double;
                    typeMap[typeof(decimal)] = DbType.Decimal;
                    typeMap[typeof(bool)] = DbType.Boolean;
                    typeMap[typeof(string)] = DbType.String;
                    typeMap[typeof(char)] = DbType.StringFixedLength;
                    typeMap[typeof(Guid)] = DbType.Guid;
                    typeMap[typeof(DateTime)] = DbType.DateTime;
                    typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
                    typeMap[typeof(byte[])] = DbType.Binary;
                    typeMap[typeof(byte?)] = DbType.Byte;
                    typeMap[typeof(sbyte?)] = DbType.SByte;
                    typeMap[typeof(short?)] = DbType.Int16;
                    typeMap[typeof(ushort?)] = DbType.UInt16;
                    typeMap[typeof(int?)] = DbType.Int32;
                    typeMap[typeof(uint?)] = DbType.UInt32;
                    typeMap[typeof(long?)] = DbType.Int64;
                    typeMap[typeof(ulong?)] = DbType.UInt64;
                    typeMap[typeof(float?)] = DbType.Single;
                    typeMap[typeof(double?)] = DbType.Double;
                    typeMap[typeof(decimal?)] = DbType.Decimal;
                    typeMap[typeof(bool?)] = DbType.Boolean;
                    typeMap[typeof(char?)] = DbType.StringFixedLength;
                    typeMap[typeof(Guid?)] = DbType.Guid;
                    typeMap[typeof(DateTime?)] = DbType.DateTime;
                    typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
                    //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
                    _typeMap = typeMap;
                }
                return _typeMap;
            }
        }


        private static DbType AsDbType(this Type type)
        {

            if (!TypeMap.ContainsKey(type))
            {
                throw new Exception("Type: [" + type.ToString() + "] not supported yet by Utilities.Sql");
            }
            var tp = TypeMap[type];
            return tp;
        }
        //private static DbType DbType<TItem>(this TItem item)
        //{
        //    var type = typeof(TItem);
        //    var tp = TypeMap[type];
        //    return tp;
        //}


        private static DbParameter CreateParamFrom(this DbCommand cmd, DbParameter param)
        {
            var p = cmd.CreateParameter();
            param.CopyInto(p);
            //p.CloneFrom(param);

            return p;
        }

        public static DbParameter Param(string name, object value)
        {

            var v = (object)value?.ToString();
            v = value;

            var p = new Parameter(name, (v ?? DBNull.Value));
            if (v != null)
            {
                p.DbType = value.GetType().AsDbType();
            }

            return p;
        }


        public static bool DoesFieldExist(DbConnection conn, string tableName, string fieldName)
        {
            string sql = "SELECT top 1 [" + fieldName + "] FROM [" + tableName + "]";
            try
            {
                var i = conn.SqlQueryScaler<object>(sql, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> DoesFieldExistAsync(DbConnection conn, string tableName, string fieldName)
        {
            string sql = "SELECT top 1 [" + fieldName + "] FROM [" + tableName + "]";
            try
            {
                var i = await conn.SqlQueryScalerAsync<object>(sql, null);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }





        public static async Task<TT> SqlQueryScalerAsync<TT>(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            try
            {

                if (parameters == null)
                {
                    parameters = new List<DbParameter>();
                }




                db.DebugWrite(sql, parameters);
                var st = DateTime.Now;
                Log("-- Executing at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                var conn = db;
                //await conn.OpenAsync(); 
                var closed = (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
                if (closed)
                {
                    Log("Opening Connection");
                    await conn.OpenAsync();
                }
                var cmd = conn.CreateCommand();
                if (!sql.ToUpper().Contains("EXEC "))
                {
                    sql = "EXEC " + sql;
                }
                if (!sql.Contains("@"))
                {
                    var fst = true;
                    foreach (var param in parameters)
                    {
                        if (!fst)
                        {
                            sql = sql + ",";
                        }
                        sql = sql + " @" + param.ParameterName;
                        //cmd.Parameters.Add(cmd.CreateParamFrom(param));
                        fst = false;
                    }
                }
                cmd.CommandText = sql;
                cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                foreach (var param in parameters)
                {

                    cmd.Parameters.Add(cmd.CreateParamFrom(param));
                }

                var ret = await cmd.ExecuteScalarAsync();
                if (closed)
                {
                    conn.Close();
                }
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                return (TT)ret;
            }
            catch (Exception ex)
            {

                var tstr = db.ArgsAsSql(sql, parameters);
                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }
        public static TT SqlQueryScaler<TT>(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            try
            {

                if (parameters == null)
                {
                    parameters = new List<DbParameter>();
                }




                db.DebugWrite(sql, parameters);
                var st = DateTime.Now;
                Log("-- Executing at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                var conn = db;
                var closed = (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
                if (closed)
                {
                    Log("Opening Connection");
                    conn.Open();
                }
                var cmd = conn.CreateCommand();
                if (!sql.ToUpper().Contains("EXEC "))
                {
                    sql = "EXEC " + sql;
                }
                if (!sql.Contains("@"))
                {
                    var fst = true;
                    foreach (var param in parameters)
                    {
                        if (!fst)
                        {
                            sql = sql + ",";
                        }
                        sql = sql + " @" + param.ParameterName;
                        //cmd.Parameters.Add(cmd.CreateParamFrom(param));
                        fst = false;
                    }
                }
                cmd.CommandText = sql;
                cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(cmd.CreateParamFrom(param));
                }

                var ret = cmd.ExecuteScalar();
                if (closed)
                {
                    conn.Close();
                }
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                return (TT)ret;
            }
            catch (Exception ex)
            {

                var tstr = db.ArgsAsSql(sql, parameters);
                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }


        public static Task<TT> SqlQueryScalerAsync<TT>(this DbConnection db, string sql, object param = null)
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }

            return db.SqlQueryScalerAsync<TT>(sql, parameters);
        }

        public static TT SqlQueryScaler<TT>(this DbConnection db, string sql, object param = null)
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }

            return db.SqlQueryScaler<TT>(sql, parameters);
        }
        #region ScalerArea

        public static async Task<TT> SqlQueryScalerAsync<TT>(this DbContext db, string sql, object param = null)
        {
            return await db.Database.GetDbConnection().SqlQueryScalerAsync<TT>(sql, param);
        }
        public static TT SqlQueryScaler<TT>(this DbContext db, string sql, object param = null)
        {
            return db.Database.GetDbConnection().SqlQueryScaler<TT>(sql, param);
        }
        public static async Task<TT> SqlQueryScalerAsync<TT>(this DatabaseFacade db, string sql, object param = null)
        {
            return await db.GetDbConnection().SqlQueryScalerAsync<TT>(sql, param);
        }
        public static TT SqlQueryScaler<TT>(this DatabaseFacade db, string sql, object param = null)
        {
            return db.GetDbConnection().SqlQueryScaler<TT>(sql, param);
        }

        public static async Task<TT> SqlQueryScalerAsync<TT>(this DbContext db, string sql, List<DbParameter> parameters)
        {
            return await db.Database.GetDbConnection().SqlQueryScalerAsync<TT>(sql, parameters);
        }
        public static TT SqlQueryScaler<TT>(this DbContext db, string sql, List<DbParameter> parameters)
        {
            return db.Database.GetDbConnection().SqlQueryScaler<TT>(sql, parameters);
        }
        public static async Task<TT> SqlQueryScalerAsync<TT>(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            return await db.GetDbConnection().SqlQueryScalerAsync<TT>(sql, parameters);
        }
        public static TT SqlQueryScaler<TT>(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            return db.GetDbConnection().SqlQueryScaler<TT>(sql, parameters);
        }
        #endregion




        public static async Task<DataSet> SqlQueryDataSetAsync(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            if (parameters == null)
            {
                parameters = new List<DbParameter>();
            }


            var st = DateTime.Now;
            Log("-- Executing at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            try
            {
                var ds = new DataSet();
                var conn = db;
                var closed = (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
                if (closed)
                {
                    Log("Opening Connection");
                    await conn.OpenAsync();
                }
                {
                    var cmd = conn.CreateCommand();
                    if (!sql.ToUpper().Contains("EXEC "))
                    {
                        sql = "EXEC " + sql;
                    }
                    if (!sql.Contains("@"))
                    {
                        var fst = true;
                        foreach (var param in parameters)
                        {
                            if (!fst)
                            {
                                sql = sql + ",";
                            }
                            sql = sql + " @" + param.ParameterName;
                            //cmd.Parameters.Add(cmd.CreateParamFrom(param));
                            fst = false;
                        }
                    }

                    db.DebugWrite(sql, parameters);

                    cmd.CommandText = sql;
                    cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(cmd.CreateParamFrom(param));
                    }

                    var reader = await cmd.ExecuteReaderAsync();
                    // Read every result set in the data reader.
                    while (!reader.IsClosed)
                    {
                        DataTable dt = new DataTable();
                        // DataTable.Load automatically advances the reader to the next result set
                        dt.Load(reader);
                        ds.Tables.Add(dt);
                    }
                    reader.Close();
                }
                if (closed)
                {
                    conn.Close();
                }
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                return ds;
            }
            catch (Exception ex)
            {
                var tstr = db.ArgsAsSql(sql, parameters);


                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }
        public static DataSet SqlQueryDataSet(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            if (parameters == null)
            {
                parameters = new List<DbParameter>();
            }

            db.DebugWrite(sql, parameters);

            var st = DateTime.Now;
            Log("-- Executing at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            try
            {
                var ds = new DataSet();
                var conn = db;
                var closed = (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
                if (closed)
                {
                    Log("Opening Connection");
                    conn.Open();
                }
                {
                    var cmd = conn.CreateCommand();
                    if (!sql.ToUpper().Contains("EXEC "))
                    {
                        sql = "EXEC " + sql;
                    }
                    if (!sql.Contains("@"))
                    {
                        var fst = true;
                        foreach (var param in parameters)
                        {
                            if (!fst)
                            {
                                sql = sql + ",";
                            }
                            sql = sql + " @" + param.ParameterName;
                            //cmd.Parameters.Add(cmd.CreateParamFrom(param));
                            fst = false;
                        }
                    }
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(cmd.CreateParamFrom(param));
                    }

                    var reader = cmd.ExecuteReader();
                    // Read every result set in the data reader.
                    while (!reader.IsClosed)
                    {
                        DataTable dt = new DataTable();
                        // DataTable.Load automatically advances the reader to the next result set
                        dt.Load(reader);
                        ds.Tables.Add(dt);
                    }
                    reader.Close();
                }
                if (closed)
                {
                    conn.Close();
                }
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                return ds;
            }
            catch (Exception ex)
            {
                var tstr = db.ArgsAsSql(sql, parameters);


                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }


        public static Task<DataSet> SqlQueryDataSetAsync(this DbConnection db, string sql, object param = null)
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }

            return db.SqlQueryDataSetAsync(sql, parameters);
        }
        public static DataSet SqlQueryDataSet(this DbConnection db, string sql, object param = null)
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
            return db.SqlQueryDataSet(sql, parameters);
        }


        #region DataSetArea

        public static async Task<DataSet> SqlQueryDataSetAsync(this DbContext db, string sql, object param = null)
        {
            return await db.Database.GetDbConnection().SqlQueryDataSetAsync(sql, param);
        }
        public static DataSet SqlQueryDataSet(this DbContext db, string sql, object param = null)
        {
            return db.Database.GetDbConnection().SqlQueryDataSet(sql, param);
        }
        public static async Task<DataSet> SqlQueryDataSetAsync(this DatabaseFacade db, string sql, object param = null)
        {
            return await db.GetDbConnection().SqlQueryDataSetAsync(sql, param);
        }
        public static DataSet SqlQueryDataSet(this DatabaseFacade db, string sql, object param = null)
        {
            return db.GetDbConnection().SqlQueryDataSet(sql, param);
        }




        public static async Task<DataSet> SqlQueryDataSetAsync(this DbContext db, string sql, List<DbParameter> parameters)
        {
            return await db.Database.GetDbConnection().SqlQueryDataSetAsync(sql, parameters);
        }
        public static DataSet SqlQueryDataSet(this DbContext db, string sql, List<DbParameter> parameters)
        {
            return db.Database.GetDbConnection().SqlQueryDataSet(sql, parameters);
        }
        public static async Task<DataSet> SqlQueryDataSetAsync(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            return await db.GetDbConnection().SqlQueryDataSetAsync(sql, parameters);
        }
        public static DataSet SqlQueryDataSet(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            return db.GetDbConnection().SqlQueryDataSet(sql, parameters);
        }

        #endregion



        #region DataTableArea

        public static async Task<DataTable> SqlQueryTableAsync(this DbConnection db, string sql, object param = null)
        {
            return (await db.SqlQueryDataSetAsync(sql, param)).Tables[0];
        }
        public static DataTable SqlQueryTable(this DbConnection db, string sql, object param = null)
        {
            return db.SqlQueryDataSet(sql, param).Tables[0];
        }
        public static async Task<DataTable> SqlQueryTableAsync(this DatabaseFacade db, string sql, object param = null)
        {
            return await db.GetDbConnection().SqlQueryTableAsync(sql, param);
        }
        public static DataTable SqlQueryTable(this DatabaseFacade db, string sql, object param = null)
        {
            return db.GetDbConnection().SqlQueryTable(sql, param);
        }
        public static async Task<DataTable> SqlQueryTableAsync(this DbContext db, string sql, object param = null)
        {
            return await db.Database.GetDbConnection().SqlQueryTableAsync(sql, param);
        }
        public static DataTable SqlQueryTable(this DbContext db, string sql, object param = null)
        {
            return db.Database.GetDbConnection().SqlQueryTable(sql, param);
        }







        public static async Task<DataTable> SqlQueryTableAsync(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            return (await db.SqlQueryDataSetAsync(sql, parameters)).Tables[0];
        }
        public static DataTable SqlQueryTable(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            return db.SqlQueryDataSet(sql, parameters).Tables[0];
        }
        public static async Task<DataTable> SqlQueryTableAsync(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            return await db.GetDbConnection().SqlQueryTableAsync(sql, parameters);
        }
        public static DataTable SqlQueryTable(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            return db.GetDbConnection().SqlQueryTable(sql, parameters);
        }
        public static async Task<DataTable> SqlQueryTableAsync(this DbContext db, string sql, List<DbParameter> parameters)
        {
            return await db.Database.GetDbConnection().SqlQueryTableAsync(sql, parameters);
        }
        public static DataTable SqlQueryTable(this DbContext db, string sql, List<DbParameter> parameters)
        {
            return db.Database.GetDbConnection().SqlQueryTable(sql, parameters);
        }


        #endregion




        public static async Task ExecuteSqlCommandAsync(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            try
            {

                if (parameters == null)
                {
                    parameters = new List<DbParameter>();
                }




                db.DebugWrite(sql, parameters);
                var st = DateTime.Now;
                Log("-- Executing at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                var conn = db;
                var closed = (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
                if (closed)
                {
                    Log("Opening Connection");
                    await conn.OpenAsync();
                }
                var cmd = conn.CreateCommand();
                if (!sql.ToUpper().Contains("EXEC "))
                {
                    sql = "EXEC " + sql;
                }
                if (!sql.Contains("@"))
                {
                    var fst = true;
                    foreach (var param in parameters)
                    {
                        if (!fst)
                        {
                            sql = sql + ",";
                        }
                        sql = sql + " @" + param.ParameterName;
                        //cmd.Parameters.Add(cmd.CreateParamFrom(param));
                        fst = false;
                    }
                }
                cmd.CommandText = sql;
                cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                foreach (var param in parameters)
                {

                    cmd.Parameters.Add(cmd.CreateParamFrom(param));
                }

                //var ret = await cmd.ExecuteNonQueryAsync();
                var ret = cmd.ExecuteNonQuery();
                if (closed)
                {
                    conn.Close();
                }
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                //return (TT)ret;
            }
            catch (Exception ex)
            {

                var tstr = db.ArgsAsSql(sql, parameters);
                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }
        public static void ExecuteSqlCommand(this DbConnection db, string sql, List<DbParameter> parameters)
        {
            try
            {

                if (parameters == null)
                {
                    parameters = new List<DbParameter>();
                }




                db.DebugWrite(sql, parameters);
                var st = DateTime.Now;
                Log("-- Executing at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                var conn = db;

                var closed = (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken);
                if (closed)
                {
                    Log("Opening Connection");
                     conn.Open();
                }
                var cmd = conn.CreateCommand();
                if (!sql.ToUpper().Contains("EXEC "))
                {
                    sql = "EXEC " + sql;
                }
                if (!sql.Contains("@"))
                {
                    var fst = true;
                    foreach (var param in parameters)
                    {
                        if (!fst)
                        {
                            sql = sql + ",";
                        }
                        sql = sql + " @" + param.ParameterName;
                        //cmd.Parameters.Add(cmd.CreateParamFrom(param));
                        fst = false;
                    }
                }
                cmd.CommandText = sql;
                cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(cmd.CreateParamFrom(param));
                }

                cmd.ExecuteNonQuery();
                if (closed)
                {
                    conn.Close();
                }
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                //return (TT)ret;
            }
            catch (Exception ex)
            {

                var tstr = db.ArgsAsSql(sql, parameters);
                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }


        public static Task ExecuteSqlCommandAsync(this DbConnection db, string sql, object param = null)
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }

            return db.ExecuteSqlCommandAsync(sql, parameters);
        }

        public static void ExecuteSqlCommand(this DbConnection db, string sql, object param = null)
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }

            db.ExecuteSqlCommand(sql, parameters);
        }


        #region SqlCommandArea


        //public static async Task ExecuteSqlCommandAsync(this DbConnection db, string sql, object param = null)
        //{
        //    var st = DateTime.Now;
        //    await db.SqlQueryDataSetAsync(sql, param);
        //    Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
        //}
        public static async Task ExecuteSqlCommandAsync(this DbContext db, string sql, object param = null)
        {
            await db.Database.GetDbConnection().ExecuteSqlCommandAsync(sql, param);
        }
        public static async Task ExecuteSqlCommandAsync(this DatabaseFacade db, string sql, object param = null)
        {
            await db.GetDbConnection().ExecuteSqlCommandAsync(sql, param);
        }



        //public static void ExecuteSqlCommand(this DbConnection db, string sql, object param = null)
        //{
        //    var st = DateTime.Now;
        //    db.ExecuteSqlCommand(sql, param);
        //    Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
        //}
        public static void ExecuteSqlCommand(this DbContext db, string sql, object param = null)
        {
            db.Database.GetDbConnection().ExecuteSqlCommand(sql, param);
        }
        public static void ExecuteSqlCommand(this DatabaseFacade db, string sql, object param = null)
        {
            db.GetDbConnection().ExecuteSqlCommand(sql, param);
        }
















        //public static async Task ExecuteSqlCommandAsync(this DbConnection db, string sql,
        //    List<DbParameter> parameters)
        //{
        //    var st = DateTime.Now;
        //    await db.SqlQueryDataSetAsync(sql, parameters);
        //    Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
        //}
        public static async Task ExecuteSqlCommandAsync(this DbContext db, string sql,
            List<DbParameter> parameters)
        {
            await db.Database.GetDbConnection().ExecuteSqlCommandAsync(sql, parameters);
        }
        public static async Task ExecuteSqlCommandAsync(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            await db.GetDbConnection().ExecuteSqlCommandAsync(sql, parameters);
        }



        //public static void ExecuteSqlCommand(this DbConnection db, string sql,
        //    List<DbParameter> parameters)
        //{
        //    var st = DateTime.Now;
        //    db.SqlQueryDataSet(sql, parameters);
        //    Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
        //}
        public static void ExecuteSqlCommand(this DbContext db, string sql,
            List<DbParameter> parameters)
        {
            db.Database.GetDbConnection().ExecuteSqlCommand(sql, parameters);
        }
        public static void ExecuteSqlCommand(this DatabaseFacade db, string sql, List<DbParameter> parameters)
        {
            db.GetDbConnection().ExecuteSqlCommand(sql, parameters);
        }

        #endregion


        #region SqlCommandParamsArea
        public static async Task ExecuteSqlCommandAsync(this DbConnection db, string sql,
            params DbParameter[] parameters)
        {
            await db.ExecuteSqlCommandAsync(sql, (from p in parameters select p).ToList());
        }
        public static async Task ExecuteSqlCommandAsync(this DatabaseFacade db, string sql,
            params DbParameter[] parameters)
        {
            await db.GetDbConnection().ExecuteSqlCommandAsync(sql, parameters);
        }
        public static async Task ExecuteSqlCommandAsync(this DbContext db, string sql,
            params DbParameter[] parameters)
        {
            await db.Database.GetDbConnection().ExecuteSqlCommandAsync(sql, parameters);
        }


        public static void ExecuteSqlCommand(this DbConnection db, string sql,
            params DbParameter[] parameters)
        {
            db.ExecuteSqlCommand(sql, (from p in parameters select p).ToList());
        }
        public static void ExecuteSqlCommand(this DatabaseFacade db, string sql,
            params DbParameter[] parameters)
        {
            db.GetDbConnection().ExecuteSqlCommand(sql, parameters);
        }
        public static void ExecuteSqlCommand(this DbContext db, string sql,
            params DbParameter[] parameters)
        {
            db.Database.GetDbConnection().ExecuteSqlCommand(sql, parameters);
        }


        #endregion


        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql,
            List<DbParameter> parameters) where TTt : class
        {
            var mappings =  Mapper.GetMapping<TTt>();
            var st = DateTime.Now;
            var tbl = await db.SqlQueryTableAsync(sql, parameters);
            var st2 = DateTime.Now;
            var q = tbl.ToList<TTt>(columnMapping: mappings).AsQueryable();
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }

        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql,
            List<DbParameter> parameters) where TTt : class
        {

            var mappings = Mapper.GetMapping<TTt>();
            var st = DateTime.Now;
            var tbl = db.SqlQueryTable(sql, parameters);
            var st2 = DateTime.Now;
            var q = tbl.ToList<TTt>(columnMapping: mappings).AsQueryable();
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }

        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(this DbConnection db,
            string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
        {
            var mappings1 = Mapper.GetMapping<TT1>();
            var mappings2 = Mapper.GetMapping<TT2>();

            var st = DateTime.Now;
            var ds = await db.SqlQueryDataSetAsync(sql, parameters);
            var st2 = DateTime.Now;
            IQueryable<TT1> q1 = null;
            IQueryable<TT2> q2 = null;
            if (ds.Tables.Count >= 1)
            {
                q1 = ds.Tables[0].ToList<TT1>(columnMapping: mappings1).AsQueryable();
            }
            if (ds.Tables.Count >= 2)
            {
                q2 = ds.Tables[1].ToList<TT2>(columnMapping: mappings2).AsQueryable();
            }
            var q = new Tuple<IQueryable<TT1>, IQueryable<TT2>>(q1, q2);
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }

        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(this DbConnection db,
            string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
        {
            var mappings1 = Mapper.GetMapping<TT1>();
            var mappings2 = Mapper.GetMapping<TT2>();
            var st = DateTime.Now;
            var ds = db.SqlQueryDataSet(sql, parameters);
            var st2 = DateTime.Now;
            IQueryable<TT1> q1 = null;
            IQueryable<TT2> q2 = null;
            if (ds.Tables.Count >= 1)
            {
                q1 = ds.Tables[0].ToList<TT1>(columnMapping: mappings1).AsQueryable();
            }
            if (ds.Tables.Count >= 2)
            {
                q2 = ds.Tables[1].ToList<TT2>(columnMapping: mappings2).AsQueryable();
            }
            var q = new Tuple<IQueryable<TT1>, IQueryable<TT2>>(q1, q2);
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }

        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(this DbConnection db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            var mappings1 = Mapper.GetMapping<TT1>();
            var mappings2 = Mapper.GetMapping<TT2>();
            var mappings3 = Mapper.GetMapping<TT3>();
            var st = DateTime.Now;
            var ds = await db.SqlQueryDataSetAsync(sql, parameters);
            var st2 = DateTime.Now;
            IQueryable<TT1> q1 = null;
            IQueryable<TT2> q2 = null;
            IQueryable<TT3> q3 = null;
            if (ds.Tables.Count >= 1)
            {
                q1 = ds.Tables[0].ToList<TT1>(columnMapping: mappings1).AsQueryable();
            }
            if (ds.Tables.Count >= 2)
            {
                q2 = ds.Tables[1].ToList<TT2>(columnMapping: mappings2).AsQueryable();
            }
            if (ds.Tables.Count >= 3)
            {
                q3 = ds.Tables[2].ToList<TT3>(columnMapping: mappings3).AsQueryable();
            }
            var q = new Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>(q1, q2, q3);
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }



        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(this DbConnection db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            var mappings1 = Mapper.GetMapping<TT1>();
            var mappings2 = Mapper.GetMapping<TT2>();
            var mappings3 = Mapper.GetMapping<TT3>();
            var st = DateTime.Now;
            var ds = db.SqlQueryDataSet(sql, parameters);
            var st2 = DateTime.Now;
            IQueryable<TT1> q1 = null;
            IQueryable<TT2> q2 = null;
            IQueryable<TT3> q3 = null;
            if (ds.Tables.Count >= 1)
            {
                q1 = ds.Tables[0].ToList<TT1>(columnMapping: mappings1).AsQueryable();
            }
            if (ds.Tables.Count >= 2)
            {
                q2 = ds.Tables[1].ToList<TT2>(columnMapping: mappings2).AsQueryable();
            }
            if (ds.Tables.Count >= 3)
            {
                q3 = ds.Tables[2].ToList<TT3>(columnMapping: mappings3).AsQueryable();
            }
            var q = new Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>(q1, q2, q3);
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }





        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql, object param = null) where TTt : class
        {
                var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
                return db.SqlQueryAsync<TTt>(sql, parameters);
        }





        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql, object param = null) where TTt : class
        {
            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
            return db.SqlQuery<TTt>(sql, parameters);

        }
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(this DbConnection db,
            string sql, object param = null)
            where TT1 : class
            where TT2 : class
        {

            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
            return db.SqlQueryAsync<TT1, TT2>(sql, parameters);
        }

        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(this DbConnection db,
            string sql, object param = null)
            where TT1 : class
            where TT2 : class
        {

            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
            return db.SqlQuery<TT1, TT2>(sql, parameters);
        }

        public static  Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(this DbConnection db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {


            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
            return db.SqlQueryAsync<TT1, TT2, TT3>(sql, parameters);
        }



        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(this DbConnection db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {

            var parameters = new List<DbParameter>();
            if (param != null)
            {
                foreach (var p in param.GetPropertyNames(onlyWritable: false))
                {
                    parameters.Add(SqlUtilities.Param(p, param.GetValue(p)));
                }
            }
            return db.SqlQuery<TT1, TT2, TT3>(sql, parameters);
        }


        #region SqlQueryArea

        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbContext db, string sql,
            object param = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TTt>(sql, param);
        }
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DatabaseFacade db, string sql, object param = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQueryAsync<TTt>(sql, param);
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DbContext db, string sql, object param = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQuery<TTt>(sql, param);
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DatabaseFacade db, string sql, object param = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQuery<TTt>(sql, param);
        }


        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DbContext db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, param);
        }
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DatabaseFacade db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, param);
        }

        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DbContext db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2>(sql, param);
        }
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DatabaseFacade db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2>(sql, param);
        }






        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DbContext db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, param);
        }
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, param);
        }






        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DbContext db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, param);
        }
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, object param = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, param);
        }

        public static  Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbContext db, string sql,
            List<DbParameter> parameters) where TTt : class
        {
            return  db.Database.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters);
        }
        public static  Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DatabaseFacade db, string sql, List<DbParameter> parameters)
            where TTt : class
        {
            return  db.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters);
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DbContext db, string sql,
            List<DbParameter> parameters) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQuery<TTt>(sql, parameters);
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DatabaseFacade db, string sql, List<DbParameter> parameters)
            where TTt : class
        {
            return db.GetDbConnection().SqlQuery<TTt>(sql, parameters);
        }


        public static  Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DbContext db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, parameters);
        }
        public static  Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, parameters);
        }

        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DbContext db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2>(sql, parameters);
        }
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2>(sql, parameters);
        }






        public static  Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DbContext db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return  db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, parameters);
        }
        public static  Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return  db.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, parameters);
        }






        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DbContext db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, parameters);
        }
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, parameters);
        }



        #endregion

        #region SqlQueryParamsArea
        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql, params DbParameter[] parameters) where TTt : class
        {
            var map = new Dictionary<string, string>();
            return db.SqlQuery<TTt>(sql, (from p in parameters select p).ToList());
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DbContext db, string sql,
            params DbParameter[] parameters) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQuery<TTt>(sql, parameters);
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DatabaseFacade db, string sql, params DbParameter[] parameters) where TTt : class
        {
            return db.GetDbConnection().SqlQuery<TTt>(sql, parameters);
        }


        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql, params DbParameter[] parameters) where TTt : class
        {
            var map = new Dictionary<string, string>();
            return await db.SqlQueryAsync<TTt>(sql, (from p in parameters select p).ToList());
        }
        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbContext db, string sql,
            params DbParameter[] parameters) where TTt : class
        {
            return await db.Database.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters);
        }
        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DatabaseFacade db, string sql, params DbParameter[] parameters) where TTt : class
        {
            return await db.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters);
        }


        #endregion

        #region ObsoleteItems

        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbContext db, string sql,
            object param = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TTt>(sql, param, mappings);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DatabaseFacade db, string sql, object param = null, Dictionary<string, string> mappings = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQueryAsync<TTt>(sql, param, mappings);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static IQueryable<TTt> SqlQuery<TTt>(this DbContext db, string sql, object param = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQuery<TTt>(sql, param, mappings);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static IQueryable<TTt> SqlQuery<TTt>(this DatabaseFacade db, string sql, object param = null, Dictionary<string, string> mappings = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQuery<TTt>(sql, param, mappings);
        }


        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DbContext db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, param, mappings1, mappings2);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DatabaseFacade db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, param, mappings1, mappings2);
        }

        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DbContext db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2>(sql, param, mappings1, mappings2);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DatabaseFacade db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2>(sql, param, mappings1, mappings2);
        }






        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DbContext db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, param, mappings1, mappings2, mappings3);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, param, mappings1, mappings2, mappings3);
        }






        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DbContext db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, param, mappings1, mappings2, mappings3);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, object param = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, param, mappings1, mappings2, mappings3);
        }

        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbContext db, string sql,
            List<DbParameter> parameters, Dictionary<string, string> mappings = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters, mappings);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DatabaseFacade db, string sql, List<DbParameter> parameters, Dictionary<string, string> mappings = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters, mappings);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static IQueryable<TTt> SqlQuery<TTt>(this DbContext db, string sql,
            List<DbParameter> parameters, Dictionary<string, string> mappings = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQuery<TTt>(sql, parameters, mappings);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static IQueryable<TTt> SqlQuery<TTt>(this DatabaseFacade db, string sql, List<DbParameter> parameters, Dictionary<string, string> mappings = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQuery<TTt>(sql, parameters, mappings);
        }


        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DbContext db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }

        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DbContext db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }






        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DbContext db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }






        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DbContext db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }



        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql, object param = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            return null;
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(this DbConnection db,
            string sql, object param = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return null;
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(this DbConnection db,
            string sql, object param = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return null;
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(this DbConnection db, string sql, object param = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null, Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return null;
        }
        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(this DbConnection db, string sql, object param = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null, Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return null;
        }






        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql,
            List<DbParameter> parameters, Dictionary<string, string> mappings) where TTt : class
        {
            return null;
        }

        [Obsolete("Use version without mapping parameter. And use Mapper.AddMapping", true)]
        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql,
            List<DbParameter> parameters, Dictionary<string, string> mappings) where TTt : class
        {
            return null;
        }
        [Obsolete("Use version without mapping parameters. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(this DbConnection db,
            string sql, List<DbParameter> parameters, Dictionary<string, string> mappings1, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return null;
        }

        [Obsolete("Use version without mapping parameters. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(this DbConnection db,
            string sql, List<DbParameter> parameters, Dictionary<string, string> mappings1, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return null;
        }

        [Obsolete("Use version without mapping parameters. And use Mapper.AddMapping", true)]
        public static Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(this DbConnection db, string sql, List<DbParameter> parameters, Dictionary<string, string> mappings1, Dictionary<string, string> mappings2 = null, Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return null;
        }


        [Obsolete("Use version without mapping parameters. And use Mapper.AddMapping", true)]
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(this DbConnection db, string sql, List<DbParameter> parameters, Dictionary<string, string> mappings1, Dictionary<string, string> mappings2 = null, Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return null;
        }


        [Obsolete("Use version without mapping parameters. And use Mapper.AddMapping", true)]
        public static Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql, object param = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            return null;
        }


        #endregion

    }
}
