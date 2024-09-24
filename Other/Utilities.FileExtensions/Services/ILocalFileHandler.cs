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

        List<string> GetDirectories(string directory, string? searchPattern = null, Func<DirectoryInfo, object>? orderBy = null, bool isAscending = false);
        List<string> GetFiles(string directory, string? searchPattern = null, Func<FileInfo, object>? orderBy = null, bool isAscending = false);


    }
}
