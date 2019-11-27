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


        public void addCookie(string name, string value, DateTime? expires, bool isPerminate)
        {
            HttpContext context = _contextAccessor.HttpContext;
            //baseData = Convert.ToBase64String(MachineKey.Protect(value,
            //        HttpContext.Current.Request.UserHostAddress));

            context.Response.Cookies.Append(
                            name,
                            value,
                            new CookieOptions()
                            {
                                Path = "/",
                                Secure = string.Equals("https", context.Request.Scheme, StringComparison.OrdinalIgnoreCase),
                                Domain = (context.Request.Host.Host.Split(".").Length > 2 ? context.Request.Host.Host : null),
                                Expires = expires
                            }
                        );
        }

        public string getCookieValue(string name)
        {
            HttpContext context = _contextAccessor.HttpContext;
            var baseData = context.Request.Cookies[name];
            //var cdata = MachineKey.Unprotect(Convert.FromBase64String(baseData), context.Request.UserHostAddress);
            return baseData;
        }
    }
}
