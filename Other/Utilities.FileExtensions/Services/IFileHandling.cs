using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions
{
    public interface IFileHandling
    {
        DateTime? GetCreatedTime(string directory, string fileName);
        DateTime? GetLastWriteTime(string directory, string fileName);
        bool Exists(string directory, string fileName);
        string GetFileURL(string directory, string fileName);
        byte[] GetFile(string directory, string fileName);
        bool Exists(string fileName);
        byte[] GetFile(string fileName);
        string GetFileURL(string fileName);
        DateTime? GetCreatedTime(string fileName);
        DateTime? GetLastWriteTime(string fileName);
        Task<bool> ExistsAsync(string directory, string fileName);
        Task<DateTime?> GetCreatedTimeAsync(string directory, string fileName);
        Task<DateTime?> GetLastWriteTimeAsync(string directory, string fileName);
        Task<byte[]> GetFileAsync(string directory, string fileName);
        Task<string> GetFileURLAsync(string directory, string fileName);
        Task<bool> ExistsAsync(string fileName);
        Task<byte[]> GetFileAsync(string fileName);
        Task<string> GetFileURLAsync(string fileName);
        Task<DateTime?> GetCreatedTimeAsync(string fileName);
        Task<DateTime?> GetLastWriteTimeAsync(string fileName);


    }
    public interface IFullFileHandling : IFileHandling
    {
        bool SaveFile(string directory, string fileName, byte[] data);
        bool SaveFile(string fileName, byte[] data);
        Task<bool> SaveFileAsync(string directory, string fileName, byte[] data);

        Task<bool> SaveFileAsync(string fileName, byte[] data);
    }

}
