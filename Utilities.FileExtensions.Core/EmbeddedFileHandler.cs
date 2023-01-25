using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileExtensions.Services;

namespace Utilities.FileExtensions.AspNetCore
{
    public class EmbeddedFileHandler<TT> : IEmbeddedFileHandling<TT>
        where TT:class
    {
        private readonly EmbeddedFileProvider efp;
        private readonly List<IFileInfo> dList;
        private readonly IServerServices _serverServices;
        private readonly Assembly assembly;

        //private readonly IServerServices _serverService;
        public EmbeddedFileHandler(IServerServices serverService)
        {
            _serverServices = serverService;
        
            assembly = Assembly.GetAssembly(typeof(TT));

            efp = new EmbeddedFileProvider(assembly);
            dList = efp.GetDirectoryContents("").ToList();

        }

        private string GetFileLocation(string directory, string fileName)
        {
            if (directory.StartsWith("~/") || directory.StartsWith("~\\"))
            {
                directory = directory.Substring(2);
            }
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }

            var fi = directory + fileName;
            fi = fi.Replace("\\", ".").Replace("/", ".").Replace("..", ".");

            return fi;
        }

        private string GetDirectoryLocation(string directory)
        {
            if (directory.StartsWith("~/") || directory.StartsWith("~\\"))
            {
                directory = directory.Substring(2);
            }
            if (!(directory.EndsWith("/") || directory.EndsWith("\\")))
            {
                directory = directory + "/";
            }

            var fi = directory;
            fi = fi.Replace("\\", ".").Replace("/", ".").Replace("..", ".");

            return fi;
        }

        private IFileInfo FindFile(string directory, string fileName)
        {
            var fi = GetFileLocation(directory, fileName).ToLower();

            var file = (from d in dList where d.Name.ToLower() == fi select d).FirstOrDefault();
            return file;
        }


        public List<string> GetDirectories(string directory)
        {
            var lst = GetEntries(directory);


            lst = lst.Where((i) => !i.Contains(".")).Distinct().ToList();

            return lst;
        }

        public List<string> GetFiles(string directory)
        {
            var lst = GetEntries(directory);


            lst = lst.Where((i) => i.Contains(".")).Distinct().ToList();

            return lst;
        }
        public List<string> GetEntries(string directory)
        {
            var dr = GetDirectoryLocation(directory);
            var drl = dr.ToLower();

            var lst = (from d in dList where d.Name.ToLower().StartsWith(drl) select d);

            var subLst = (from i in lst select i.Name.Replace(dr, "", true, null)).ToList();


            var finLst = (from i in subLst select i.Split("."));

            var fLst = (from i2 in finLst select (i2.Count() > 2 ? i2[0] : i2[0] + "." + i2[1])).ToList();


            return fLst;
        }

        public bool Exists(string directory, string fileName)
        {
            var file = FindFile(directory, fileName);


            return file != null;
        }

        public byte[] GetFile(string directory, string fileName)
        {
            var file = FindFile(directory, fileName);

            if (file != null)
            {
                var resourceStream = assembly.GetManifestResourceStream(file.Name);


                using (var reader = new  BinaryReader(resourceStream))
                {
                    return reader.ReadBytes((int)resourceStream.Length);
                }

            }
            return null;
        }

        public string GetFileURL(string directory, string fileName)
        {
            var file = FindFile(directory, fileName);
            if (file !=null)
            {
                var fName = file.Name;
                return _serverServices.GetUrl("~" + Core.GetEmbeddedBasePath(assembly)  + "/" + fName);
            }
            return null;
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

        public DateTime? GetCreatedTime(string directory, string fileName)
        {
            var file = FindFile(directory, fileName);
            return file?.LastModified.DateTime;
        }

        public DateTime? GetLastWriteTime(string directory, string fileName)
        {
            var file = FindFile(directory, fileName);
            return file?.LastModified.DateTime;
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


        public Task<bool> ExistsAsync(string directory, string fileName)
        {
            return Task.FromResult(Exists(directory, fileName));
        }


        public Task<byte[]> GetFileAsync(string directory, string fileName)
        {
            return Task.FromResult(GetFile(directory, fileName));
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


        public Task<string> GetFileURLAsync(string fileName)
        {
            return Task.FromResult(GetFileURL(fileName));
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

    }
}
