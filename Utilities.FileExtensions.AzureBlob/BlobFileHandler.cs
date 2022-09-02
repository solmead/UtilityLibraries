using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.AzureBlob
{
    public class BlobFileHandler : IFullFileHandling
    {
        private readonly ILogger _logger;
        private readonly BlobOptions _options;
        private readonly BlobServiceClient _blobServiceClient;

        private Dictionary<string, CloudBlobDirectory> _directories = new Dictionary<string, CloudBlobDirectory>();

        public BlobFileHandler(BlobOptions options, ILogger logger)
        {
            _logger = logger;
            _options = options;
            _blobServiceClient = new BlobServiceClient(options.ConnectionString);
        }
        private CloudBlobDirectory GetDirectoryBlob(string directory)
        {
            if (directory == null) directory = "";
            directory = directory.Replace('\\', '/');
            if (directory.EndsWith("/") || directory.EndsWith("\\"))
            {
                directory = directory.Substring(0, directory.Length - 1);
            }
            directory = directory.ToLower().Replace("_", "-");

            if (directory.StartsWith("~"))
            {
                directory = directory.Substring(1);
            }

            if (_directories.ContainsKey(directory.ToLower()))
            {
                var dir = _directories[directory.ToLower()];
                return dir;
            }

            try
            {
                var tstr = directory.Split('/').ToList();
                foreach (var t in tstr.ToArray())
                {
                    if (string.IsNullOrWhiteSpace(t))
                    {
                        tstr.Remove(t);
                    }
                }
                while (tstr.Count < 3)
                {
                    tstr.Add("base");
                }



                var containerClient = _blobServiceClient.GetBlobContainerClient(tstr[1].Trim().ToLower());
                //var containerClient =  _storageAccount.CreateBlobContainer(tstr[1].Trim().ToLower());

                var blobContainerInfo = containerClient.CreateIfNotExists();

                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);


                CloudBlobDirectory dir = containerClient.GetDirectoryReference(tstr[2].Trim());

                for (var a = 3; a < tstr.Count; a++)
                {
                    dir = dir.GetDirectoryReference(tstr[a].Trim());
                }

                _directories.Add(directory.ToLower(), dir);


                return dir;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                var a = 1;
                //ex.LogToElmah();
            }
            return null;

        }
        private async Task<CloudBlobDirectory> GetDirectoryBlobAsync(string directory)
        {

            if (directory == null) directory = "";
            directory = directory.Replace('\\', '/');
            if (directory.EndsWith("/") || directory.EndsWith("\\"))
            {
                directory = directory.Substring(0, directory.Length - 1);
            }
            directory = directory.ToLower().Replace("_", "-");
            var tstr = directory.Split('/').ToList();

            if (directory.StartsWith("~"))
            {
                directory = directory.Substring(1);
            }


            if (_directories.ContainsKey(directory.ToLower()))
            {
                var dir = _directories[directory.ToLower()];
                return dir;
            }

            try
            {
                foreach (var t in tstr.ToArray())
                {
                    if (string.IsNullOrWhiteSpace(t))
                    {
                        tstr.Remove(t);
                    }
                }
                while (tstr.Count < 3)
                {
                    tstr.Add("base");
                }

                var containerClient = _blobServiceClient.GetBlobContainerClient(tstr[1].Trim().ToLower());

                //var containerClient = await _blobServiceClient.GetBlobContainersAsync(prefix: "");
                    
                    
                    //(tstr[1].Trim().ToLower());
                //var containerClient = await _storageAccount.CreateBlobContainerAsync(tstr[1].Trim().ToLower());

                var blobContainerInfo = await containerClient.CreateIfNotExistsAsync();

                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);


                CloudBlobDirectory dir = containerClient.GetDirectoryReference(tstr[2].Trim());

                for (var a = 3; a < tstr.Count; a++)
                {
                    dir = dir.GetDirectoryReference(tstr[a].Trim());
                }

                _directories.Add(directory.ToLower(), dir);


                return dir;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                var a = 1;
                //ex.LogToElmah();
            }
            return null;

        }

        public bool Exists(string directory, string fileName)
        {
            


            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient = dir.GetBlobClient(fileName);
            //var blockBlob = dir.GetBlockBlobReference(fileName);
            return blobClient.Exists();
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

        public byte[] GetFile(string directory, string fileName)
        {
            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = dir.GetBlockBlobReference(fileName);
            var blobClient = dir.GetBlobClient(fileName);

            var mStream = new MemoryStream();

            blobClient.DownloadTo(mStream);
            mStream.Seek(0, SeekOrigin.Begin);
            return mStream.ToArray();
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

        public FileInfo GetFileInfo(string directory, string fileName)
        {
            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = dir.GetBlockBlobReference(fileName);
            var blobClient = dir.GetBlobClient(fileName);

            //blobClient.
            return null;
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

        public DateTime? GetCreatedTime(string directory, string fileName)
        {
            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient = dir.GetBlobClient(fileName);
            if (blobClient.Exists())
            {
                var properties = blobClient.GetProperties().Value;
                return properties.CreatedOn.DateTime;
            } else
            {
                return null;
            }
        }

        public DateTime? GetLastWriteTime(string directory, string fileName)
        {
            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient =  dir.GetBlobClient(fileName);
            if (blobClient.Exists())
            {
                var properties = blobClient.GetProperties().Value;
                return properties.LastModified.DateTime;
            } else
            {
                return null;
            }
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


        public string GetFileURL(string directory, string fileName)
        {
            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = dir.GetBlockBlobReference(fileName);
            var blobClient = dir.GetBlobClient(fileName);
            return blobClient.Uri.ToString();



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

        public bool SaveFile(string directory, string fileName, byte[] data)
        {
            var dir = GetDirectoryBlob(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient = dir.GetBlobClient(fileName);
            blobClient.DeleteIfExists();
            var bd = new BinaryData(data);
            blobClient.Upload(bd);
            //CloudBlockBlob blockBlob = dir.GetBlockBlobReference(fileName);
            //await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
            return true;
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

        public async Task<bool> ExistsAsync(string directory, string fileName)
        {
            //_logger.LogInformation("ExistsAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient = await dir.GetBlobClientAsync(fileName);
            //var blockBlob = dir.GetBlockBlobReference(fileName);
            return await blobClient.ExistsAsync();
        }

        public async Task<FileInfo> GetFileInfoAsync(string directory, string fileName)
        {
            //_logger.LogInformation("GetFileInfoAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = await dir.GetBlockBlobReferenceAsync(fileName);
            var blobClient = await dir.GetBlobClientAsync(fileName);


            

            //blobClient.
            return null;
        }

        public async Task<byte[]> GetFileAsync(string directory, string fileName)
        {
            //_logger.LogInformation("GetFileAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = await dir.GetBlockBlobReferenceAsync(fileName);
            var blobClient = await dir.GetBlobClientAsync(fileName);

            var mStream = new MemoryStream();

            await blobClient.DownloadToAsync(mStream);
            mStream.Seek(0, SeekOrigin.Begin);
            return mStream.ToArray();
        }

        public async Task<bool> SaveFileAsync(string directory, string fileName, byte[] data)
        {
            //_logger.LogInformation("SaveFileAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient = await dir.GetBlobClientAsync(fileName);
            await blobClient.DeleteIfExistsAsync();
            var bd = new BinaryData(data);
            await blobClient.UploadAsync(bd);
            return true;
        }

        public async Task<string> GetFileURLAsync(string directory, string fileName)
        {
            //_logger.LogInformation("GetFileURLAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = dir.GetBlockBlobReference(fileName);
            var blobClient = await dir.GetBlobClientAsync(fileName);
            return blobClient.Uri.ToString();
        }

        public Task<bool> ExistsAsync(string fileName)
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
            return ExistsAsync(dir, fileName);
        }

        public Task<byte[]> GetFileAsync(string fileName)
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
            return GetFileAsync(dir, fileName);
        }

        public Task<bool> SaveFileAsync(string fileName, byte[] data)
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
            return SaveFileAsync(dir, fileName, data);
        }

        public Task<string> GetFileURLAsync(string fileName)
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
            return GetFileURLAsync(dir, fileName);
        }

        public Task<FileInfo> GetFileInfoAsync(string fileName)
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
            return GetFileInfoAsync(dir, fileName);
        }

        public async Task<DateTime?> GetCreatedTimeAsync(string directory, string fileName)
        {
            //_logger.LogInformation("GetFileInfoAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            //var blockBlob = await dir.GetBlockBlobReferenceAsync(fileName);
            var blobClient = await dir.GetBlobClientAsync(fileName);
            if (await blobClient.ExistsAsync()) { 
                var properties = blobClient.GetProperties().Value;
                return properties.CreatedOn.DateTime;
            } else
            {
                return null;
            }
        }

        public async Task<DateTime?> GetLastWriteTimeAsync(string directory, string fileName)
        {
            //_logger.LogInformation("GetFileInfoAsync called for dir=[" + directory + "] name=[" + fileName + "]");
            var dir = await GetDirectoryBlobAsync(directory);
            if (dir == null)
            {
                throw new Exception("Directory Blob not found");
            }
            var blobClient = await dir.GetBlobClientAsync(fileName);
            if (await blobClient.ExistsAsync())
            {
                var properties = blobClient.GetProperties().Value;
                return properties.LastModified.DateTime;
            }
            else
            {
                return null;
            }
        }

        public Task<DateTime?> GetCreatedTimeAsync(string fileName)
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
            return GetCreatedTimeAsync(dir, fileName);
        }

        public Task<DateTime?> GetLastWriteTimeAsync(string fileName)
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
            return GetLastWriteTimeAsync(dir, fileName);
        }
    }
}
