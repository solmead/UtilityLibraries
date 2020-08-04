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
        void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait);
        void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait);
        Task SavePdfFromUrlAsync(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait);
        Byte[] GetPdfFromHtml(string html, string baseUrl, PageOrientation orientation = PageOrientation.Portrait);
        byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait);
        Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait);

        void CombineFiles(List<FileInfo> fileList, FileInfo toFile);

        Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile);
    }
}
