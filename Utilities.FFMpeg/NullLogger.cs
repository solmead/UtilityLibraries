using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.MediaConverter
{
    internal class NullLogger : ILogger,IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
            //throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //throw new NotImplementedException();
        }
    }
}
