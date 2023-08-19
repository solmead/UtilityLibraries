using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using Utilities.Dapper.TypeHandlers;

namespace Utilities.Dapper
{
    internal class Class1
    {
        public void todo()
        {
            var conn = new LoggedSqlConnection(null, null);

            var s = conn.SqlQueryAsync<FallBackTypeMapper, FallBackTypeMapper>("", new { i=0});
        }
    }
}
