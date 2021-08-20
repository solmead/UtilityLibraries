using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Caching.Core
{
    public interface ICacheCookie
    {


        void ResetCookieId();
        Task<string> CookieIdAsync();
        string CookieId();
    }
}
