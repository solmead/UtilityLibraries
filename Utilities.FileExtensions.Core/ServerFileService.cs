
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
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

        private Uri BaseUrl(IUrlHelper urlHelper)
        {
            var baseUrl = "";

            //No configuration given, so use the one from the context
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                var request = _httpContextAccessor.HttpContext.Request;
                baseUrl = request.Scheme + "://" + request.Host.Value;
            }

            return new Uri(baseUrl);
        }

        public string GetAbsUrl(string path)
        {
            if (path.Contains("://"))
            {
                return path;
            }
            if (_actionContextAccessor?.ActionContext == null)
            {
                _logger.LogError("No Context available on _actionContextAccessor");
                return "";
            }

            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                return path;
            }

            Uri combinedUri;
            if (Uri.TryCreate(BaseUrl(urlHelper), path, out combinedUri))
            {
                return combinedUri.AbsoluteUri;
            }

            throw new Exception(string.Format("Could not create absolute url for {0} using baseUri{0}", path, BaseUrl(urlHelper)));
        }

        public string GetTempDirectory()
        {
            return "/Temp/";
        }

        public string GetUrl(string path)
        {
            try
            {
                if (_actionContextAccessor?.ActionContext == null)
                {
                    _logger.LogError("No Context available on _actionContextAccessor");
                    return "";
                }

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
            if (_actionContextAccessor?.ActionContext == null)
            {
                _logger.LogError("No Context available on _actionContextAccessor");
                return "";
            }
            if (_httpContextAccessor?.HttpContext == null)
            {
                _logger.LogError("No Context available on _httpContextAccessor");
                return "";
            }

            IUrlHelper url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var actionUrl = url.Action(action, parameters);

            //var baseUrl = UriHelper.GetDisplayUrl(_httpContextAccessor.HttpContext.Request);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = request.Scheme + "://" + request.Host.Value;


            return baseUrl + actionUrl;
        }

        public string GetUrlRoute(string route, object parameters)
        {
            if (_actionContextAccessor?.ActionContext == null)
            {
                _logger.LogError("No Context available on _actionContextAccessor");
                return "";
            }
            IUrlHelper url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var actionUrl = url.RouteUrl(route, parameters);

            // var baseUrl = UriHelper.GetDisplayUrl(_httpContextAccessor.HttpContext.Request);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = request.Scheme + "://" + request.Host.Value;

            return baseUrl + actionUrl;
        }

        public string MapPath(string path)
        {
            //_logger.LogInformation("MapPath path=[" + path + "]");
            path = path.Trim();
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);

            var dirPath = "" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            var netPath = "" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;


            if (path.StartsWith(netPath) || path.Contains(dirPath))
            {
                //_logger.LogInformation("MapPath network or full path=[" + path + "]");
                return path;
            }

            path = path.Replace(netPath, "" + Path.DirectorySeparatorChar);


            if (path?.FirstOrDefault() == '~')
            {
                //_logger.LogInformation("MapPath starts with ~");
                path = path.Substring(1);
            }

            if (path?.FirstOrDefault() != Path.DirectorySeparatorChar)
            {
                path = Path.DirectorySeparatorChar + path;
            }


            //_logger.LogInformation("MapPath checking path=[" + path + "]");

            var pths = path.Split(Path.DirectorySeparatorChar);

            var subPath = "";
            for(var a=2; a<pths.Length; a++)
            {
                subPath = subPath + Path.DirectorySeparatorChar + pths[a] ;
            }


            //_logger.LogInformation("MapPath looking for mapping for [" + pths[1] + "]");

            var pt = _fileServerProvider.GetProvider(pths[1]) as PhysicalFileProvider;
            var pt2 = _fileServerProvider.GetProvider("/" + pths[1]) as PhysicalFileProvider;
            var pt3 = _fileServerProvider.GetProvider("\\" + pths[1]) as PhysicalFileProvider;

            //if (path.Contains("documents") && pt==null)
            //{
            //    throw new Exception("Chris expected documents to be mapped to a PhysicalFileProvider pths[1]=[" + pths[1] + "] pt2 exists = [" + (pt2!=null) + "]");
            //}


            //_logger.LogInformation("MapPath pt = " + (pt==null ? "is null" : "found provider"));
            //_logger.LogInformation("MapPath pt2 = " + (pt2 == null ? "is null" : "found provider"));
            //_logger.LogInformation("MapPath pt3 = " + (pt3 == null ? "is null" : "found provider"));

            pt = pt ?? pt2 ?? pt3;

            var finPath = "";
            if (pt != null)
            {
                finPath = Path.GetFullPath(Path.Join(pt.Root, subPath));

            } else
            {
                string contentRootPath = _webHostEnvironment.ContentRootPath;
                //_logger.LogInformation("MapPath contentRootPath = [" + contentRootPath + "]");
                finPath = Path.GetFullPath(Path.Join(contentRootPath, path));

            }

            //_logger.LogInformation("MapPath finPath = [" + finPath + "]");
            return finPath;
           
        }
    }
}


