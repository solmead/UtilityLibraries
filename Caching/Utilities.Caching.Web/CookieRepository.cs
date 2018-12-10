using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Utilities.Caching.Core;

namespace Utilities.Caching.Web
{
    public class CookieRepository : ICookieRepository
    { 
        
        public CookieRepository()
        {

        }


        public void addCookie(string name, string value, DateTime? expires, bool isPerminate)
        {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = null;
            try
            {
                cookie = context.Request.Cookies[name];
            }
            catch (Exception)
            {

            }

            try
            {
                if (cookie == null)
                {
                    cookie = new HttpCookie(name, value);
                }
                cookie.Value = value;
                //cookie.HttpOnly = true;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                cookie.Secure = string.Equals("https", HttpContext.Current.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase);
                if (expires.HasValue)
                {
                    cookie.Expires = expires.Value;
                }
                if (isPerminate)
                {
                    cookie.Expires = DateTime.Now.AddYears(3);
                }
                if (HttpContext.Current.Request.Url.Host.Split('.').Length > 2)
                {
                    cookie.Domain = HttpContext.Current.Request.Url.Host;
                }
                context.Response.SetCookie(cookie);

            }
            catch (Exception)
            {

            }

        }

        public string getCookieValue(string name)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                HttpCookie cookie = null;
                try
                {
                    cookie = context.Request.Cookies[name];
                }
                catch (Exception)
                {

                }
                return cookie?.Value;

            }
            return null;
        }
    }
}
