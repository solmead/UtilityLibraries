using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Caching.Core;

namespace Utilities.Caching.AspNetCore
{
    public class CookieRepository : ICookieRepository
    {

        private IHttpContextAccessor _contextAccessor;

        public CookieRepository(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private static string _cookiePath = null;

        public static string CookiePath
        {
            get
            {
                //FormsAuthentication.FormsCookiePath;
                if (string.IsNullOrWhiteSpace(_cookiePath))
                {
                    _cookiePath = "/";
                    //_cookiePath = System.Web.VirtualPathUtility.ToAbsolute("~/");
                }


                return _cookiePath;
            }
            set
            {
                _cookiePath = value;
            }
        }

        public void addCookie(string name, string value, DateTime? expires, bool isPerminate)
        {
            HttpContext context = _contextAccessor.HttpContext;
            //baseData = Convert.ToBase64String(MachineKey.Protect(value,
            //        HttpContext.Current.Request.UserHostAddress));

            Cache.SetItem(CacheArea.Request, "Cookie_" + name, value);

           //ResponseCookie cookie = null;

           // try
           // {
           //     cookie = context.Response.Cookies.[name];
           // }
           // catch (Exception)
           // {

           // }
           // try
           // {
           //     if (cookie == null)
           //     {
           //         cookie = context.Request.Cookies[name];
           //     }
           // }
           // catch (Exception)
           // {

           // }
            var co = new CookieOptions()
            {
                Path = CookiePath,
                Secure = string.Equals("https", context.Request.Scheme, StringComparison.OrdinalIgnoreCase),
                Domain = (context.Request.Host.Host.Split(".").Length > 2 ? context.Request.Host.Host : null)
            };
            if (expires.HasValue)
            {
                co.Expires = expires.Value;
            }
            if (isPerminate)
            {
                co.Expires = DateTime.Now.AddYears(3);
            }


            context.Response.Cookies.Append(
                            name,
                            value,
                            co
                        );
        }

        public string getCookieValue(string name)
        {
            return Cache.GetItem(CacheArea.Request, "Cookie_" + name, () =>
            {
                HttpContext context = _contextAccessor.HttpContext;
                var baseData = context?.Request?.Cookies[name];
                //var cdata = MachineKey.Unprotect(Convert.FromBase64String(baseData), context.Request.UserHostAddress);
                return baseData;
            });
        }


        public void clearCookie(string name)
        {
            HttpContext context = _contextAccessor.HttpContext;

            Cache.SetItem(CacheArea.Request, "Cookie_" + name, "");
            try
            {
                var _response = context.Response;
                _response.Cookies.Append(name, "", new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(-1)
                });

            }
            catch (Exception)
            {

            }
        }
    }
}
