using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileExtensions.Services;

namespace Utilities.FileExtensions
{
    public class LocalFileHandler : IFileHandling, ILocalFileHandling, IFullFileHandling
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
        public DateTime? GetCreatedTime(string directory, string fileName)
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
            return fi.CreationTime;
        }

        public DateTime? GetLastWriteTime(string directory, string fileName)
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
            return fi.LastWriteTime;
        }

        public DateTime? GetCreatedTime(string fileName)
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
            return GetCreatedTime(dir, fileName);
        }

        public DateTime? GetLastWriteTime(string fileName)
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
            return GetLastWriteTime(dir, fileName);
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







        public Task<bool> ExistsAsync(string directory, string fileName)
        {
            return Task.FromResult(Exists(directory, fileName));
        }

        public Task<FileInfo> GetFileInfoAsync(string directory, string fileName)
        {
            return Task.FromResult(GetFileInfo(directory, fileName));
        }

        public Task<byte[]> GetFileAsync(string directory, string fileName)
        {
            return Task.FromResult(GetFile(directory, fileName));
        }

        public Task<bool> SaveFileAsync(string directory, string fileName, byte[] data)
        {
            return Task.FromResult(SaveFile(directory, fileName, data));
        }

        public Task<string> GetFileURLAsync(string directory, string fileName)
        {
            return Task.FromResult(GetFileURL(directory, fileName));
        }

        public Task<bool> ExistsAsync(string fileName)
        {
            return Task.FromResult(Exists(fileName));
        }

        public Task<byte[]> GetFileAsync(string fileName)
        {
            return Task.FromResult(GetFile(fileName));
        }

        public Task<bool> SaveFileAsync(string fileName, byte[] data)
        {
            return Task.FromResult(SaveFile(fileName, data));
        }

        public Task<string> GetFileURLAsync(string fileName)
        {
            return Task.FromResult(GetFileURL(fileName));
        }

        public Task<FileInfo> GetFileInfoAsync(string fileName)
        {
            return Task.FromResult(GetFileInfo(fileName));
        }

        
        public Task<DateTime?> GetCreatedTimeAsync(string directory, string fileName)
        {
            return Task.FromResult(GetCreatedTime(directory, fileName));
        }

        public Task<DateTime?> GetLastWriteTimeAsync(string directory, string fileName)
        {
            return Task.FromResult(GetLastWriteTime(directory, fileName));
        }

        public Task<DateTime?> GetCreatedTimeAsync(string fileName)
        {
            return Task.FromResult(GetCreatedTime(fileName));
        }

        public Task<DateTime?> GetLastWriteTimeAsync(string fileName)
        {
            return Task.FromResult(GetLastWriteTime(fileName));
        }

        public List<string> GetDirectories(string directory)
        {

            directory = directory.Replace("\\", "/");
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }
            var di = new DirectoryInfo(_serverService.MapPath(directory));
            if (!di.Exists)
            {
                di.Create();
            }


            return  di.GetDirectories().Select((d)=>d.Name).ToList();



        }

        public List<string> GetFiles(string directory)
        {
            directory = directory.Replace("\\", "/");
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }
            var di = new DirectoryInfo(_serverService.MapPath(directory));
            if (!di.Exists)
            {
                di.Create();
            }


            return di.GetFiles().Select((d) => d.Name).ToList();
        }
    }
}
