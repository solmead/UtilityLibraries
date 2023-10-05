using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Utilities.Dapper
{
    public static class Core
    {

        public static IEnumerable<TT1> SqlQuery<TT1>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {

            if (commandType ==null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = db.Query<TT1>(sql, param, transaction, buffered, commandTimeout, commandType);


            var q = items;

            return q;
        }
        public static Tuple<IEnumerable<TT1>, IEnumerable<TT2>> SqlQuery<TT1, TT2>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = db.QueryMultiple(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>>(q1, q2);

            return q;
        }

        public static Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>> SqlQuery<TT1, TT2, TT3>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = db.QueryMultiple(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();
            var q3 = items.Read<TT3>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>>(q1, q2, q3);

            return q;
        }

        public static Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>> SqlQuery<TT1, TT2, TT3, TT4>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
            where TT4 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = db.QueryMultiple(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();
            var q3 = items.Read<TT3>();
            var q4 = items.Read<TT4>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>>(q1, q2, q3, q4);

            return q;
        }

        public static async Task ExecuteSqlCommandAsync(this IDbConnection db,
           string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);


        }
        public static async Task<TT> SqlQueryScalerAsync<TT>(this DbConnection db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) where TT : struct
        {
            return (await SqlQueryAsync<TT>(db, sql, param, transaction, commandTimeout, commandType)).FirstOrDefault();
        }
        public static async Task<TT> SqlQuerySingleAsync<TT>(this DbConnection db, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return (await SqlQueryAsync<TT>(db, sql, param, transaction, commandTimeout, commandType)).FirstOrDefault();
        }


        public static async Task<IEnumerable<TT1>> SqlQueryAsync<TT1>(this IDbConnection db,
           string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.QueryAsync<TT1>(sql, param, transaction, commandTimeout, commandType);


            var q = items;

            return q;
        }
        public static async Task<Tuple<IEnumerable<TT1>, IEnumerable<TT2>>> SqlQueryAsync<TT1, TT2>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>>(q1, q2);

            return q;
        }

        public static async Task<Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>>> SqlQueryAsync<TT1, TT2, TT3>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null,  int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();
            var q3 = items.Read<TT3>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>>(q1, q2, q3);

            return q;
        }

        public static async Task<Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>>> SqlQueryAsync<TT1, TT2, TT3, TT4>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null,  int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
            where TT4 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();
            var q3 = items.Read<TT3>();
            var q4 = items.Read<TT4>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>>(q1, q2, q3, q4);

            return q;
        }

        public static async Task<Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>, IEnumerable<TT5>>> SqlQueryAsync<TT1, TT2, TT3, TT4, TT5>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
            where TT4 : class
            where TT5 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();
            var q3 = items.Read<TT3>();
            var q4 = items.Read<TT4>();
            var q5 = items.Read<TT5>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>, IEnumerable<TT5>>(q1, q2, q3, q4, q5);

            return q;
        }

        public static async Task<Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>, IEnumerable<TT5>, IEnumerable<TT6>>> SqlQueryAsync<TT1, TT2, TT3, TT4, TT5, TT6>(this IDbConnection db,
            string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            where TT1 : class
            where TT2 : class
            where TT3 : class
            where TT4 : class
            where TT5 : class
            where TT6 : class
        {

            if (commandType == null && !sql.ToUpper().StartsWith("EXEC ") && !sql.ToUpper().Contains(" @"))
            {
                commandType = CommandType.StoredProcedure;
            }


            var items = await db.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

            var q1 = items.Read<TT1>();
            var q2 = items.Read<TT2>();
            var q3 = items.Read<TT3>();
            var q4 = items.Read<TT4>();
            var q5 = items.Read<TT5>();
            var q6 = items.Read<TT6>();

            var q = new Tuple<IEnumerable<TT1>, IEnumerable<TT2>, IEnumerable<TT3>, IEnumerable<TT4>, IEnumerable<TT5>, IEnumerable<TT6>>(q1, q2, q3, q4, q5, q6);

            return q;
        }





    }
}
