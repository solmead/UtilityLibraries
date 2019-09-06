using NLog;
using NLog.Targets;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.Logging
{
    public static class Startup
    {

        public static void Init(ILogUserRepository userRepo = null)
        {
            Log.userRepo = userRepo;

            var t = new Task(Log.CleanLogging);
            t.Start();
            //CleanLogging();

        }

        public static void Init(string logPath, ILogUserRepository userRepo)
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
                fi = new FileInfo(userRepo.MapPath(logPath + "/logFile.txt"));
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                fTarget.ArchiveFileName = fi.FullName.Replace(".txt", ".{#}.txt");
                fTarget.FileName = fi.FullName;
                Init(userRepo);

            } catch(Exception ex)
            {
                var msg = "Error: " + ex.Message + " logPath = [" + logPath + "] fi=[" + fi?.FullName + "]";
                throw new Exception(msg, ex);
            }

        }
    }
}
