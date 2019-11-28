
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
    }
}
