using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities.FileExtensions.Services;

namespace Utilities.FileExtensions
{
    public class LocalFileHandler : IFileHandling
    {
        private readonly IServerServices _serverService;
        public LocalFileHandler(IServerServices serverService)
        {
            _serverService = serverService;
        }

        //public static string BaseUrl()
        //{
        //    return Cache.GetItem<string>(CacheArea.Global, "BaseURL", ()=> {
        //        if (HttpContext.Current != null)
        //        {

        //            var request = HttpContext.Current.Request;
        //            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

        //            if (!String.IsNullOrWhiteSpace(appUrl) && (appUrl.Last() != '/')) appUrl += "/";
        //            return String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
        //        }
        //        return null;
        //    });
        //}



        public bool Exists(string directory, string fileName)
        {
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }
            var fi = new FileInfo(_serverService.MapPath(directory + fileName));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            return fi.Exists;
        }

        public byte[] GetFile(string directory, string fileName)
        {
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }
            var fi = new FileInfo(_serverService.MapPath(directory + fileName));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            if (fi.Exists)
            {
                return File.ReadAllBytes(fi.FullName);
            }
            return null;
        }

        public bool SaveFile(string directory, string fileName, byte[] data)
        {
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }
            var fi = new FileInfo(_serverService.MapPath(directory + fileName));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            if (fi.Exists)
            {
                fi.Delete();
            }
            File.WriteAllBytes(fi.FullName, data);
            return true;
        }

        public string GetFileURL(string directory, string fileName)
        {
            if (directory.StartsWith("~/") || directory.StartsWith("~\\"))
            {
                directory = directory.Substring(2);
            }

            return _serverService.GetUrl("~/" + directory + fileName);
        }

        public bool Exists(string fileName)
        {
            fileName = fileName.Replace("\\", "/");
            var last = fileName.LastIndexOf("/");
            var dir = "";
            if (last >= 0)
            {
                last = last + 1;
                dir = fileName.Substring(0, last);
                fileName = fileName.Substring(last);
            }
            return Exists(dir, fileName);
        }

        public byte[] GetFile(string fileName)
        {
            fileName = fileName.Replace("\\", "/");
            var last = fileName.LastIndexOf("/");
            var dir = "";
            if (last >= 0)
            {
                last = last + 1;
                dir = fileName.Substring(0, last);
                fileName = fileName.Substring(last);
            }
            return GetFile(dir, fileName);
        }

        public bool SaveFile(string fileName, byte[] data)
        {
            fileName = fileName.Replace("\\", "/");
            var last = fileName.LastIndexOf("/");
            var dir = "";
            if (last >= 0)
            {
                last = last + 1;
                dir = fileName.Substring(0, last);
                fileName = fileName.Substring(last);
            }
            return SaveFile(dir, fileName, data);
        }

        public string GetFileURL(string fileName)
        {
            fileName = fileName.Replace("\\", "/");
            var last = fileName.LastIndexOf("/");
            var dir = "";
            if (last >= 0)
            {
                last = last + 1;
                dir = fileName.Substring(0, last);
                fileName = fileName.Substring(last);
            }
            return GetFileURL(dir, fileName);
        }

        public FileInfo GetFileInfo(string directory, string fileName)
        {
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }
            var fi = new FileInfo(_serverService.MapPath(directory + fileName));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            return fi;

        }

        public FileInfo GetFileInfo(string fileName)
        {



            fileName = fileName.Replace("\\", "/");
            var last = fileName.LastIndexOf("/");
            var dir = "";
            if (last >= 0)
            {
                last = last + 1;
                dir = fileName.Substring(0, last);
                fileName = fileName.Substring(last);
            }
            return GetFileInfo(dir, fileName);
        }
    }
}
