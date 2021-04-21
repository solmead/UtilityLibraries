using Microsoft.Extensions.Logging;
using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    public static class Startup
    {
        public static FileInfo logLocation;
        private static void InitSystem(ILogUserRepository userRepo = null)
        {
            Log.userRepo = userRepo;

            var t = new Task(() => { Log.CleanLogging(); });
            t.Start();
            //CleanLogging();

        }
        public static void Init(ILogUserRepository userRepo = null)
        {
            var mf = Log.GetConfiguredMainLogFile();
            mf = mf.Replace("/", "\\");
            if (!mf.StartsWith("\\") && !mf.Contains(":\\") && !mf.StartsWith("~"))
            {
                mf = "~\\" + mf;
            }

            var pos = mf.LastIndexOf("\\");

            var dir = mf.Substring(0, pos+1);
            var fname = mf.Substring(pos + 1);

            Init(dir, userRepo, fname);
        }

        public static void Init(string logPath, ILogUserRepository userRepo, string fileName = "logFile.txt")
        {
            FileInfo fi = null;
            try
            {
                Log.userRepo = userRepo;

                var target = LogManager.Configuration.FindTargetByName("asyncFile") as NLog.Targets.Wrappers.AsyncTargetWrapper;

                var fTarget = target.WrappedTarget as FileTarget;

                if (string.IsNullOrWhiteSpace(logPath))
                {
                    logPath = "/Logs/";
                }

                logPath = logPath.Replace("\\", "/");

                //\\itdata.ad.uc.edu\EAS\applications\production\webapps2\grantsdashboard\Documents\logs\logFile.txt
                //\\itdata.ad.uc.edu\EAS\applications\production\webapps2\grantsdashboard\Documents\logs\logFile.{#}.txt
                //            return System.Web.Hosting.HostingEnvironment.MapPath(basePath);

                //var di = new DirectoryInfo(HostingEnvironment.MapPath("/Documents/Logs/"));
                fi = new FileInfo(userRepo.MapPath(logPath + "/" + fileName));
                logLocation = fi;
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                fTarget.ArchiveFileName = fi.FullName.Replace(fi.Extension, ".{#}" + fi.Extension);
                fTarget.FileName = fi.FullName;
                InitSystem(userRepo);


                //Microsoft.Extensions.Logging.ILogger _logger = new GenericLogger();
                //_logger.LogInformation("Finished Logger Setup");


            } catch(Exception ex)
            {
                var msg = "Error: " + ex.Message + " logPath = [" + logPath + "] fi=[" + fi?.FullName + "]";
                
                throw new Exception(msg, ex);
            }

            Microsoft.Extensions.Logging.ILogger _logger = new GenericLogger();


            _logger.LogInformation("Information Log");
            _logger.LogWarning("Warning Log");
            _logger.LogCritical("Critical Log");
            _logger.LogDebug("Debug Log");
            _logger.LogError("Error Log");
            _logger.LogTrace("Trace Log");
        }
    }
}
