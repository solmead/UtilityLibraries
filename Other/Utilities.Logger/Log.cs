using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Utilities.Logging
{
    public static class Log
    {
        public static ILogUserRepository userRepo { get; set; }


        public static Logger Logger { get; set; }

        private static Logger _logger
        {
            get
            {
                if (Logger == null)
                {
                    Logger = LogManager.GetCurrentClassLogger();
                }


                if (!isLoaded)
                {
                    isLoaded = true;


                    //LogManager.LoadConfiguration("Content/NLog.config");
                    //var logFac = logger.Factory.LoadConfiguration("Content/NLog.config");
                }

                return Logger;
            }
        }
        private static bool isLoaded = false;

        public static void AddProperty(string name, string value)
        {
            Logger = _logger.WithProperty(name, value);
        }



        //private static readonly ILog logger = LogProvider.For();

        private static string MachineName()
        {
            

            return Environment.MachineName;
        }

        private static string CurrentUser()
        {
            var host = userRepo?.UserHostAddress();
            var name = userRepo?.CurrentUserName();
            return "{" + (name ?? host ?? "Unknown") + "}";
        }

        private static string getMessage(string msg)
        {

            var name = CurrentUser();
            var now = DateTime.Now;
            var st = String.Format(" {0} | {1} | {2}", MachineName(), name, msg);
            return st;
        }

        public static void Trace(string msg)
        {
            _logger.Trace(getMessage(msg));
        }
        public static void Debug(string msg)
        {
            _logger.Debug(getMessage(msg));
        }
        public static void Info(string msg)
        {
            _logger.Info(getMessage(msg));
        }
        public static void Warn(string msg)
        {
            _logger.Warn(getMessage(msg));
        }
        public static void Error(Exception ex)
        {
            _logger.Error(getMessage(ex.ToString()));
        }
        public static void Error(string msg)
        {
            _logger.Error(getMessage(msg));
        }
        public static void Fatal(string msg)
        {
            _logger.Fatal(getMessage(msg));
        }
        private static string GetLogFileName(string targetName)
        {
            string fileName = null;

            if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
            {
                Target target = LogManager.Configuration.FindTargetByName(targetName);
                if (target == null)
                {
                    throw new Exception("Could not find target named: " + targetName);
                }

                FileTarget fileTarget = null;
                WrapperTargetBase wrapperTarget = target as WrapperTargetBase;

                // Unwrap the target if necessary.
                if (wrapperTarget == null)
                {
                    fileTarget = target as FileTarget;
                }
                else
                {
                    fileTarget = wrapperTarget.WrappedTarget as FileTarget;
                }

                if (fileTarget == null)
                {
                    throw new Exception("Could not get a FileTarget from " + target.GetType());
                }

                var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                fileName = fileTarget.FileName.Render(logEventInfo);
            }
            else
            {
                throw new Exception("LogManager contains no Configuration or there are no named targets");
            }

            if (!File.Exists(fileName))
            {
                throw new Exception("File " + fileName + " does not exist");
            }

            return fileName;
        }



        public static async void CleanLogging()
        {
            await Task.Delay(10000);
            try
            {
                Log.Info("CleanLogs - Started");
                var file = new FileInfo(GetLogFileName("asyncFile"));
                var dir = file.Directory;
                if (!dir.Exists)
                {
                    dir.Create();
                }

                var refDate = DateTime.Now.AddMonths(-1); //-Settings.Default.MaxMonthsToKeep);
                var files = (from fi in dir.GetFiles() where (fi.CreationTime <= refDate) select fi).ToList();
                Log.Info(("CleanLogs - " + (files.Count + " found")));
                foreach (var f in files)
                {
                    try
                    {
                        f.Delete();
                    }
                    catch (Exception ex)
                    {
                    }

                }

                Log.Info("CleanLogs - Finished");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
