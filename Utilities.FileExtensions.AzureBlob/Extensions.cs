using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.FileExtensions.AzureBlob
{
    internal static class Extensions
    {


        public static CloudBlobDirectory GetDirectoryReference(this BlobContainerClient client, string directory)
        {
            return new CloudBlobDirectory(client, "/", directory);
        }

        public static CloudBlobDirectory GetDirectoryReference(this CloudBlobDirectory client, string directory)
        {
            return new CloudBlobDirectory(client.containerClient, client.directoryDelimiter, client.directory + client.directoryDelimiter + directory);

        }

    }
}
