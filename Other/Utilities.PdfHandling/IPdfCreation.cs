using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.PdfHandling
{
    public enum PageOrientation
    {
        Portrait,
        Landscape
    }
    public interface IPdfCreation
    {
        //[Obsolete("No longer supported", true)]
        //void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait);
        //[Obsolete("Switch to using Async version of calls -> SavePdfFromUrlAsync", true)]
        //void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait);
        //[Obsolete("No longer supported", true)]
        //Byte[] GetPdfFromHtml(string html, string baseUrl, PageOrientation orientation = PageOrientation.Portrait);
        //[Obsolete("Switch to using Async version of calls -> GetPdfFromUrlAsync", true)]
        //byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait);

        //[Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        //void CombineFiles(List<FileInfo> fileList, FileInfo toFile);

        //[Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        //Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile);
        //[Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        //FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName);




        Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName);
        Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait);
        Task SavePdfFromUrlAsync(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait);



    }
}
