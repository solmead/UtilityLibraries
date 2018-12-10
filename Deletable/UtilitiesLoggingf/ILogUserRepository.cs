using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Logging
{
    public interface ILogUserRepository
    {
        string CurrentUserName();
        string UserHostAddress();
    }
}
