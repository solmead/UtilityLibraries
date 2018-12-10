using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Caching.Core
{
    public interface ICookieRepository
    {
        string getCookieValue(string name);
        void addCookie(string name, string value, DateTime? expires, bool isPerminate);

    }
}
