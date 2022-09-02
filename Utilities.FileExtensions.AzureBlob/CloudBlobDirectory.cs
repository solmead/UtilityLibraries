using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.AzureBlob
{
    internal class CloudBlobDirectory
    {
        public BlobContainerClient containerClient { get; set; }

        public string directoryDelimiter { get; set; }
        public string directory { get; set; }

        public CloudBlobDirectory(BlobContainerClient client, string delimiter, string directory)
        {

            if (directory == null) directory = "";
            directory = directory.Replace("\\", delimiter);
            directory = directory.Replace("/", delimiter);
            if (directory.EndsWith(delimiter))
            {
                directory = directory.Substring(0, directory.Length - 1);
            }
            

            containerClient = client;
            directoryDelimiter = delimiter;
            this.directory = directory.ToLower();
        }

        public BlobClient GetBlobClient(string fileName)
        {
            var blobClient = containerClient.GetBlobClient(directory + directoryDelimiter + fileName.ToLower());
            return blobClient;

        }

        public BlobItem GetBlockBlobReference(string filename)
        {


            var blobs = containerClient.GetBlobsByHierarchy(BlobTraits.All, BlobStates.All, directoryDelimiter, directory);
            var blob = blobs.Where((b) => {
                return b.Blob.Name.ToLower() == filename.ToLower();
            }).FirstOrDefault();
            
            return blob?.Blob;
        }

        public Task<BlobClient> GetBlobClientAsync(string fileName)
        {
            

            var blobClient =  containerClient.GetBlobClient(directory + directoryDelimiter + fileName.ToLower());
            return Task.FromResult(blobClient);

        }

        public async Task<BlobItem> GetBlockBlobReferenceAsync(string filename)
        {
            var blobs = containerClient.GetBlobsByHierarchyAsync(BlobTraits.All, BlobStates.All, directoryDelimiter, directory);


            var blob = await blobs.Where((b) => {
                return b.Blob.Name.ToLower() == filename.ToLower();
            }).FirstOrDefaultAsync();

            return blob?.Blob;
        }
    }
}
