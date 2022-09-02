
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities.FileExtensions.Services;

namespace Utilities.FileExtensions.AspNetCore
{
    public class ServerFileServices : IServerServices
    {
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IPhysicalFileServerProvider _fileServerProvider;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ServerFileServices> _logger;

        public ServerFileServices(IWebHostEnvironment webHostEnvironment,
            IUrlHelperFactory urlHelperFactory,
                   IActionContextAccessor actionContextAccessor,
                   IHttpContextAccessor httpContextAccessor,
                   IPhysicalFileServerProvider fileServerProvider,
                   ILogger<ServerFileServices> logger)
        {
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
            _fileServerProvider = fileServerProvider;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string GetTempDirectory()
        {
            return "/Temp/";
        }

        public string GetUrl(string path)
        {
            try
            {
                IUrlHelper url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var actionUrl = url.Content(path);
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = request.Scheme + "://" + request.Host.Value;


                return baseUrl + actionUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                return "";
            }
        }

        public string GetUrl(string action, object parameters)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var actionUrl = url.Action(action, parameters);

            //var baseUrl = UriHelper.GetDisplayUrl(_httpContextAccessor.HttpContext.Request);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = request.Scheme + "://" + request.Host.Value;


            return baseUrl + actionUrl;
        }

        public string GetUrlRoute(string route, object parameters)
        {
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var actionUrl = url.RouteUrl(route, parameters);

            // var baseUrl = UriHelper.GetDisplayUrl(_httpContextAccessor.HttpContext.Request);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = request.Scheme + "://" + request.Host.Value;

            return baseUrl + actionUrl;
        }

        public string MapPath(string path)
        {
            path = path.Trim();
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);

            var dirPath = "" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            var netPath = "" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;


            if (path.StartsWith(netPath) || path.Contains(dirPath))
            {
                return path;
            }

            path = path.Replace(netPath, "" + Path.DirectorySeparatorChar);


            if (path.First() == '~')
            {
                path = path.Substring(1);
            }

            if (path.First() != Path.DirectorySeparatorChar)
            {
                path = Path.DirectorySeparatorChar + path;
            }

            var pths = path.Split(Path.DirectorySeparatorChar);
            var pt = _fileServerProvider.GetProvider(pths[1]) as PhysicalFileProvider;


            if (pt != null)
            {
                return Path.Join(pt.Root, path);
                //if (_siteSettings.DocumentsDirectory.Contains(netPath) || _siteSettings.DocumentsDirectory.Contains("." + Path.DirectorySeparatorChar) || _siteSettings.DocumentsDirectory.Contains(dirPath))
                //{
                //    path = path.Substring(10);
                //    var finPath = Path.Join(_siteSettings.DocumentsDirectory, path);
                //    return finPath;
                //}
            }
            //path.Contains(".\\") ||



            //if (_siteSettings.DocumentsPath.Contains("\\\\") || _siteSettings.DocumentsPath.Contains(".\\") || _siteSettings.DocumentsPath.Contains(":\\"))
            //{
            //    //app.UseFileServer(new FileServerOptions
            //    //{
            //    //    FileProvider = new PhysicalFileProvider(settings.DocumentsPath),
            //    //    RequestPath = new PathString("/Documents"),
            //    //    EnableDirectoryBrowsing = false
            //    //});
            //}





            //string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            //string path = "";
            //path = Path.Combine(webRootPath, "CSS");



            return Path.Join(contentRootPath, path);

            //throw new NotImplementedException();
            //throw new NotImplementedException();
        }
    }
}


