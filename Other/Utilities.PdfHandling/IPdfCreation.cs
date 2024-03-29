﻿using System;
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

        [Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)")]
        void CombineFiles(List<FileInfo> fileList, FileInfo toFile);

        [Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)")]
        Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile);

        Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName);
        FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName);



    }
}
