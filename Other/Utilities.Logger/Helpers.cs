using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public static class Helpers
    {

        public static void LogToElmah(this Exception ex)
        {
            Log.Error(ex);
        }
    }
}
