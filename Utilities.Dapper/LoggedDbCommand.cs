using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Utilities.Dapper
{
    public class LoggedDbCommand : DbCommand
    {
        private readonly ILogger _logger;
        //private readonly Guid _correlationId;
        private DbCommand _command;
        private bool _disposed;

        public DbLog WhatToLog { get; set; } = DbLog.All;

        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }


        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }


        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }


        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }


        protected override DbConnection DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }


        protected override DbParameterCollection DbParameterCollection => _command.Parameters;


        protected override DbTransaction DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }


        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }


        public LoggedDbCommand(ILogger Logger, DbCommand Command)
        {
            _logger = Logger;
            //_correlationId = CorrelationId;
            _command = Command;
        }


        ~LoggedDbCommand() => Dispose(false);


        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // No managed resources to release.
            }
            // Release unmanaged resources.
            _command?.Dispose();
            _command = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }


        public override void Cancel()
        {
            if (WhatToLog.HasFlag(DbLog.Command))
            {
                _logger.LogDebug("Cancelling database command.");
            }
            _command.Cancel();
        }


        public override int ExecuteNonQuery()
        {
            LogCommandBeforeExecuted();
            int result = _command.ExecuteNonQuery();
            LogCommandAfterExecuted();
            return result;
        }


        public override object ExecuteScalar()
        {
            LogCommandBeforeExecuted();
            return _command.ExecuteScalar();
        }


        public override void Prepare()
        {
            if (WhatToLog.HasFlag(DbLog.Command))
            {
                _logger.LogDebug("Preparing database command.");
            }
            _command.Prepare();
        }


        protected override DbParameter CreateDbParameter() => _command.CreateParameter();


        protected override DbDataReader ExecuteDbDataReader(CommandBehavior Behavior)
        {
            LogCommandBeforeExecuted();
            return _command.ExecuteReader(Behavior);
        }


        private void LogCommandBeforeExecuted()
        {
            if (WhatToLog.HasFlag(DbLog.SqlCall))
            {

                var pList = new List<DbParameter>();
                foreach (DbParameter p in _command.Parameters)
                {
                    pList.Add(p);
                }


                _logger.LogDebug(_command.Connection.ArgsAsSql(_command.CommandText, pList));
            }

            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine($"Database command type = {_command.CommandType}");
            //stringBuilder.AppendLine($"Database command text = {_command.CommandText}.");
            //foreach (IDataParameter parameter in _command.Parameters)
            //{
            //    if ((parameter.Direction == ParameterDirection.Output) ||
            //      (parameter.Direction == ParameterDirection.ReturnValue)) continue;
            //    stringBuilder.AppendLine
            //      ($"Database command parameter {parameter.ParameterName} = {parameter.Value}.");
            //}
            //_logger.LogDebug(stringBuilder.ToString());
        }


        private void LogCommandAfterExecuted()
        {
            if (WhatToLog.HasFlag(DbLog.SqlCall))
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (IDataParameter parameter in _command.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Input) continue;
                    stringBuilder.AppendLine
                      ($"Database command parameter {parameter.ParameterName} = {parameter.Value}.");
                }
                _logger.LogDebug(stringBuilder.ToString());
            }
        }
    }
}
