using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public class GenericLogger : ILogger
    {
        private readonly ILogger _logger;

        public GenericLogger(ILogger logger)
        {
            _logger = logger;
        }
        public GenericLogger()
        {
            var lg = new NLog.Extensions.Logging.NLogLoggerFactory();
            var lgg = lg.CreateLogger("");
            _logger = lgg;
        }



        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope<TState>(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            _logger.Log(logLevel, eventId, state, exception, (TState st, Exception ex) =>
            {
                var f = formatter(st, ex);
                var msg = Utilities.Logging.Log.getMessage(f);
                return msg;
            });

            //throw new NotImplementedException();
        }
    }
}
