
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.AspNetCore
{
    public static class FileExt
    {

        public static bool SaveFile(this IFullFileHandling fileHandlng, IFormFile file, string path, string filename = null)
        {
            filename = filename ?? file.FileName;
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Seek(0, SeekOrigin.Begin);
                fileHandlng.SaveFile(path, filename, stream);
                return true;
            }
        }

        public static Task<bool> SaveFileAsync(this IFormFile file, IFullFileHandling fileHandlng, string path, string filename = null)
        {
            return Task.FromResult(SaveFile(fileHandlng, file, path, filename));
        }
        public static bool SaveFile(this IFormFile file, IFullFileHandling fileHandlng, string path, string filename = null)
        {

            return SaveFile(fileHandlng, file, path, filename);
        }

        public static Task<bool> SaveFileAsync(this IFullFileHandling fileHandlng, IFormFile file, string path, string filename = null)
        {
            return Task.FromResult(SaveFile(fileHandlng, file, path, filename));
        }
    }
}
