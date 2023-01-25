using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.Services
{
    public interface ILocalFileHandling : IFullFileHandling
    {
        FileInfo GetFileInfo(string fileName);
        FileInfo GetFileInfo(string directory, string fileName);
        Task<FileInfo> GetFileInfoAsync(string directory, string fileName);
        Task<FileInfo> GetFileInfoAsync(string fileName);

        List<string> GetDirectories(string directory);
        List<string> GetFiles(string directory);


    }
}
