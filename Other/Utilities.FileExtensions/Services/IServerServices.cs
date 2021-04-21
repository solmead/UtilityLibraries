using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FileExtensions.Services
{
    public interface IServerServices
    {
        string GetTempDirectory();

        string MapPath(string path);
        //string BaseUrl();
        string GetUrl(string path);
        string GetUrl(string action, object parameters);
        string GetUrlRoute(string route, object parameters);

    }
}
