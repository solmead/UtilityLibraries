using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Utilities.Dapper
{
    public class LoggedSqlConnection : LoggedDbConnection
    {
        public LoggedSqlConnection(ILogger Logger, string Connection) : base(Logger)
        {
            this.Connection = new SqlConnection(Connection);
        }
    }
}
