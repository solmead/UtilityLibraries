using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.Caching.Core;

namespace Utilities.Caching.Helpers
{
    public class CacheCookies : ICookieRepository, ICacheCookie
    {
        private readonly ICookieRepository _cookieRepository;

        public CacheCookies(ICookieRepository cookieRepository)
        {
            _cookieRepository = cookieRepository;
        }

        public void addCookie(string name, string value, DateTime? expires, bool isPerminate)
        {
            _cookieRepository.addCookie(name, value, expires, isPerminate);
        }

        public void clearCookie(string name)
        {

            Cache.SetItem<string>(CacheArea.Request, "Cookie_" + name + "_Id", null);

            _cookieRepository.clearCookie("_" + name + "_Caching");
        }
        public string getCookieValue(string name)
        {
            return getCookieValue(name, true);
        }
        public string getCookieValue(string name, bool isPerminate)
        {

            return Cache.GetItem<string>(CacheArea.Request, "Cookie_" + name + "_Id", () =>
            {
                if (_cookieRepository == null)
                {
                    throw new Exception("CookieRepository not initialized");
                }
                //try
                //{
                string cookie = _cookieRepository.getCookieValue("_" + name + "_Caching");
                if (string.IsNullOrWhiteSpace(cookie))
                {
                    cookie = Guid.NewGuid().ToString();
                    _cookieRepository.addCookie("_" + name + "_Caching", cookie, (isPerminate ? DateTime.Now.AddYears(3) : (DateTime?)null), isPerminate);

                }

                //}
                //catch (Exception)
                //{

                //}
                return cookie;
            });

        }






        public void ResetCookieId()
        {
            clearCookie("my_cook");
        }
        public async Task<string> CookieIdAsync()
        {
            return getCookieValue("my_cook", true);
        }

        public string CookieId()
        {
            return getCookieValue("my_cook", true);
        }
    }
}
