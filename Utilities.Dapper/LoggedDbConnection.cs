using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Utilities.Dapper
{
    [Flags]
    public enum DbLog
    {
        None = 0,
        Connection = 1,
        Command = 2,
        SqlCall = 4,
        All = 7
    }

    public abstract class LoggedDbConnection : DbConnection
    {
        protected DbConnection Connection;
        private readonly ILogger _logger;
        //private readonly Guid _correlationId;
        private bool _disposed;

        public DbLog WhatToLog { get; set; } = DbLog.All;

        public override string ConnectionString
        {
            get => Connection.ConnectionString;
            set => Connection.ConnectionString = value;
        }


        public override string Database => Connection.Database;


        public override string DataSource => Connection.DataSource;


        public override string ServerVersion => Connection.ServerVersion;


        public override ConnectionState State => Connection.State;


        protected LoggedDbConnection(ILogger Logger)
        {
            _logger = Logger;
            //_correlationId = CorrelationId;
        }


        ~LoggedDbConnection() => Dispose(false);


        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // No managed resources to release.
            }
            // Release unmanaged resources.
            Connection?.Dispose();
            Connection = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }


        public override void ChangeDatabase(string DatabaseName)
        {
            if (WhatToLog.HasFlag(DbLog.Connection))
            {
                _logger.LogDebug($"Changing database to {DatabaseName}.");
            }
            Connection.ChangeDatabase(DatabaseName);
        }


        public override void Close()
        {
            if (WhatToLog.HasFlag(DbLog.Connection))
            {
                _logger.LogDebug($"Closing database connection to {Connection.ConnectionString}.");
            }
            Connection.Close();
        }


        public override void Open()
        {
            if (WhatToLog.HasFlag(DbLog.Connection))
            {
                _logger.LogDebug($"Opening database connection to {Connection.ConnectionString}.");
            }
            Connection.Open();
        }


        protected override DbTransaction BeginDbTransaction(IsolationLevel IsolationLevel)
        {
            if (WhatToLog.HasFlag(DbLog.Connection))
            {
                _logger.LogDebug($"Beginning database transaction with IsolationLevel = {IsolationLevel}.");
            }
            return Connection.BeginTransaction();
        }


        protected override DbCommand CreateDbCommand()
        {
            if (WhatToLog.HasFlag(DbLog.Connection))
            {
                _logger.LogDebug("Creating database command.");
            }
            var cmd =  new LoggedDbCommand(_logger, Connection.CreateCommand());
            cmd.WhatToLog = WhatToLog;
            return cmd;
        }
    }
}
