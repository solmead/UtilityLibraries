using System;
using Elmah;

namespace Utilities.ElmahExtensions
{
    public static class ElmahExtension
    {
        public static void LogToElmah(this Exception ex)
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                else
                {
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                }
            }
            catch (Exception ex2)
            {
                
            }
        }

        private static HttpApplication httpApplication = null;
        private static ErrorFilterConsole errorFilter = new ErrorFilterConsole();

        public static ErrorMailModule ErrorEmail = new ErrorMailModule();
        public static ErrorLogModule ErrorLog = new ErrorLogModule();
        public static ErrorTweetModule ErrorTweet = new ErrorTweetModule();

        private static void InitNoContext()
        {
            httpApplication = new HttpApplication();
            errorFilter.Init(httpApplication);

            (ErrorEmail as IHttpModule).Init(httpApplication);
            errorFilter.HookFiltering(ErrorEmail);
            ErrorEmail.Mailing += ErrorEmailOnMailing;

            (ErrorLog as IHttpModule).Init(httpApplication);
            errorFilter.HookFiltering(ErrorLog);

            (ErrorTweet as IHttpModule).Init(httpApplication);
            errorFilter.HookFiltering(ErrorTweet);
        }

        private static void ErrorEmailOnMailing(object sender, ErrorMailEventArgs e)
        {
            e.Mail.Subject = "Error on - " + "Main" + ": " + e.Mail.Subject;
            //Log.TraceError(e.Error.ToString());
        }

        private class ErrorFilterConsole : ErrorFilterModule
        {
            protected override void OnErrorModuleFiltering(object sender, ExceptionFilterEventArgs args)
            {
                base.OnErrorModuleFiltering(sender, args);
                
            }

            public void HookFiltering(IExceptionFiltering module)
            {
                module.Filtering += new ExceptionFilterEventHandler(OnErrorModuleFiltering);
            }
        }

    }
}
