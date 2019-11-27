using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Utilities.Poco;

namespace Utilities.Sql
{
    public static class SqlUtilities
    {

        public static Action<string> LogMessage { get; set; }

        public static void Log(string msg)
        {
            //Debug.WriteLine(msg);
            LogMessage?.Invoke(msg);
        }

        private static DbParameter CreateParamFrom(this DbCommand cmd, DbParameter param)
        {
            var p = cmd.CreateParameter();
            param.CopyInto(p);
            //p.CloneFrom(param);

            return p;
        }
        
        public static DbParameter Param(string name, object value)
        {
            return new Parameter(name, (value ?? DBNull.Value));
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


        public static async Task ExecuteSqlCommandAsync(this DbConnection db, string sql,
            List<DbParameter> parameters = null)
        {
            var st = DateTime.Now;
            await db.SqlQueryDataSetAsync(sql, parameters);
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
        }
        public static async Task ExecuteSqlCommandAsync(this DbContext db, string sql,
            List<DbParameter> parameters = null)
        {
            await db.Database.GetDbConnection().ExecuteSqlCommandAsync(sql, parameters);
        }
        public static async Task ExecuteSqlCommandAsync(this DatabaseFacade db, string sql, List<DbParameter> parameters = null)
        {
            await db.GetDbConnection().ExecuteSqlCommandAsync(sql, parameters);
        }



        public static void ExecuteSqlCommand(this DbConnection db, string sql,
            List<DbParameter> parameters = null)
        {
            var st = DateTime.Now;
            db.SqlQueryDataSet(sql, parameters);
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
        }
        public static void ExecuteSqlCommand(this DbContext db, string sql,
            List<DbParameter> parameters = null)
        {
            db.Database.GetDbConnection().ExecuteSqlCommand(sql, parameters);
        }
        public static void ExecuteSqlCommand(this DatabaseFacade db, string sql, List<DbParameter> parameters = null)
        {
            db.GetDbConnection().ExecuteSqlCommand(sql, parameters);
        }

        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql,
            List<DbParameter> parameters = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            if (mappings == null)
            {
                mappings = new Dictionary<string, string>();
            }
            var st = DateTime.Now;
            var tbl = await db.SqlQueryTableAsync(sql, parameters);
            var st2 = DateTime.Now;
            var q = tbl.ToList<TTt>(columnMapping: mappings).AsQueryable();
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }

        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbContext db, string sql,
            List<DbParameter> parameters = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            return await db.Database.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters, mappings);
        }
        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DatabaseFacade db, string sql, List<DbParameter> parameters = null, Dictionary<string, string> mappings = null)
            where TTt : class
        {
            return await db.GetDbConnection().SqlQueryAsync<TTt>(sql, parameters, mappings);
        }

        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql,
            List<DbParameter> parameters = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            if (mappings == null)
            {
                mappings = new Dictionary<string, string>();
            }
            var st = DateTime.Now;
            var tbl = db.SqlQueryTable(sql, parameters);
            var st2 = DateTime.Now;
            var q = tbl.ToList<TTt>(columnMapping: mappings).AsQueryable();
            Log("-- Mapping in " + DateTime.Now.Subtract(st2).TotalMilliseconds + " ms");
            Log("-- Completed in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
            return q;
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DbContext db, string sql,
            List<DbParameter> parameters = null, Dictionary<string, string> mappings = null) where TTt : class
        {
            return db.Database.GetDbConnection().SqlQuery<TTt>(sql, parameters, mappings);
        }
        public static IQueryable<TTt> SqlQuery<TTt>(this DatabaseFacade db, string sql, List<DbParameter> parameters = null, Dictionary<string, string> mappings = null)
            where TTt : class
        {
            return db.GetDbConnection().SqlQuery<TTt>(sql, parameters, mappings);
        }


        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DbContext db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }

        public static Tuple<IQueryable<TT1>, IQueryable<TT2>> SqlQuery<TT1, TT2>(this DbConnection db,
            string sql, List<DbParameter> parameters = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class 
            where TT2 : class
        {
            mappings1 = mappings1 ?? new Dictionary<string, string>();
            mappings2 = mappings2 ?? new Dictionary<string, string>();
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


        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DbContext db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.Database.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }
        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return db.GetDbConnection().SqlQuery<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }


        public static Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>> SqlQuery<TT1, TT2, TT3>(this DbConnection db, string sql, List<DbParameter> parameters = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null, Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            mappings1 = mappings1 ?? new Dictionary<string, string>();
            mappings2 = mappings2 ?? new Dictionary<string, string>();
            mappings3 = mappings3 ?? new Dictionary<string, string>();
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





        public static IQueryable<TTt> SqlQuery<TTt>(this DbConnection db, string sql, params DbParameter[] parameters) where TTt : class
        {
            var map = new Dictionary<string, string>();
            return db.SqlQuery<TTt>(sql, (from p in parameters select p).ToList(), map);
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










        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DbContext db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return await db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }
        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            return await db.GetDbConnection().SqlQueryAsync<TT1, TT2>(sql, parameters, mappings1, mappings2);
        }

        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>>> SqlQueryAsync<TT1, TT2>(this DbConnection db,
            string sql, List<DbParameter> parameters = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null)
            where TT1 : class
            where TT2 : class
        {
            mappings1 = mappings1 ?? new Dictionary<string, string>();
            mappings2 = mappings2 ?? new Dictionary<string, string>();
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


        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DbContext db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return await db.Database.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }
        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(
            this DatabaseFacade db, string sql, List<DbParameter> parameters = null,
            Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null,
            Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            return await db.GetDbConnection().SqlQueryAsync<TT1, TT2, TT3>(sql, parameters, mappings1, mappings2, mappings3);
        }


        public static async Task<Tuple<IQueryable<TT1>, IQueryable<TT2>, IQueryable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(this DbConnection db, string sql, List<DbParameter> parameters = null, Dictionary<string, string> mappings1 = null, Dictionary<string, string> mappings2 = null, Dictionary<string, string> mappings3 = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {
            mappings1 = mappings1 ?? new Dictionary<string, string>();
            mappings2 = mappings2 ?? new Dictionary<string, string>();
            mappings3 = mappings3 ?? new Dictionary<string, string>();
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





        public static async Task<IQueryable<TTt>> SqlQueryAsync<TTt>(this DbConnection db, string sql, params DbParameter[] parameters) where TTt : class
        {
            var map = new Dictionary<string, string>();
            return await db.SqlQueryAsync<TTt>(sql, (from p in parameters select p).ToList(), map);
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






        public static async Task<DataSet> SqlQueryDataSetAsync(this DbConnection db, string sql, List<DbParameter> parameters)
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
                    await conn.OpenAsync();
                }
                {
                    var cmd = conn.CreateCommand();
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
                await conn.OpenAsync();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                foreach (var param in parameters)
                {

                    cmd.Parameters.Add(cmd.CreateParamFrom(param));
                }

                var ret = await cmd.ExecuteScalarAsync();
                conn.Close();
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

                 conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandTimeout = (db.ConnectionTimeout != 0 ? db.ConnectionTimeout : 20);

                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(cmd.CreateParamFrom(param));
                }

                var ret = cmd.ExecuteScalar();
                conn.Close();
                Log("-- Loaded in " + DateTime.Now.Subtract(st).TotalMilliseconds + " ms");
                return (TT)ret;
            }
            catch (Exception ex)
            {

                var tstr = db.ArgsAsSql(sql, parameters);
                throw new Exception("Error: " + ex.Message + " on db call:" + tstr, ex);
            }
        }


    }
}
