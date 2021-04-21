using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utilities.FileExtensions
{
    public interface IFileHandling
    {
        bool Exists(string directory, string fileName);
        FileInfo GetFileInfo(string directory, string fileName);
        byte[] GetFile(string directory, string fileName);
        bool SaveFile(string directory, string fileName, byte[] data);
        string GetFileURL(string directory, string fileName);
        bool Exists(string fileName);
        byte[] GetFile(string fileName);
        bool SaveFile(string fileName, byte[] data);
        string GetFileURL(string fileName);
        FileInfo GetFileInfo(string fileName);
    }
}
