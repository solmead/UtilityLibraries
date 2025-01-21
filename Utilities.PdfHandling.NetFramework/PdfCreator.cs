//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;

//namespace Utilities.PdfHandling.NetFramework
//{
//    [Obsolete("No longer used, now setup when Configurator inits called", true)]
//    public class PdfCreator : IPdfCreation
//    {


//        private static string HiQPDFSerial { get; set; } = "DUVkXF1p-a0Fkb39s-f3Q8PSM9-LTwtPy01-PDUtPjwj-PD8jNDQ0-NA==";

//        private readonly ILogger _logger;
//        public PdfCreator(ILogger logger)
//        {
//            _logger = logger;
//        }

//        //public void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
//        //{
//        //    //Log("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
//        //    if (!file.Directory.Exists)
//        //    {
//        //        file.Directory.Create();
//        //    }
//        //    if (file.Exists)
//        //    {
//        //        file.Delete();
//        //    }
//        //    var data = GetPdfFromHtml(html, baseUrl, orientation);
//        //    if (data != null)
//        //    {
//        //        var st = file.OpenWrite();
//        //        st.Write(data, 0, data.Length);
//        //        st.Close();
//        //    }
//        //    file.Refresh();


//        //    //Log("SavePdfFromUrl End");
//        //}

//        //public void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
//        //{
//        //    //Log("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
//        //    if (!file.Directory.Exists)
//        //    {
//        //        file.Directory.Create();
//        //    }
//        //    if (file.Exists)
//        //    {
//        //        file.Delete();
//        //    }
//        //    var data = GetPdfFromUrl(url, orientation);
//        //    if (data != null)
//        //    {
//        //        var st = file.OpenWrite();
//        //        st.Write(data, 0, data.Length);
//        //        st.Close();
//        //    }
//        //    file.Refresh();


//        //    //Log("SavePdfFromUrl End");
//        //}

//        //public async Task SavePdfFromUrlAsync(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
//        //{
//        //    //Log("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
//        //    if (!file.Directory.Exists)
//        //    {
//        //        file.Directory.Create();
//        //    }
//        //    if (file.Exists)
//        //    {
//        //        file.Delete();
//        //    }
//        //    var data = await GetPdfFromUrlAsync(url, orientation);
//        //    if (data != null)
//        //    {
//        //        var st = file.OpenWrite();
//        //        st.Write(data, 0, data.Length);
//        //        st.Close();
//        //    }
//        //    file.Refresh();


//        //    //Log("SavePdfFromUrl End");
//        //}

//        //public byte[] GetPdfFromHtml(string html, string baseUrl, PageOrientation orientation = PageOrientation.Portrait)
//        //{
//        //    return null;
//        //}
//        //public byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait)
//        //{
//        //    return null;
//        //}
//        //public Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
//        //{
//        //    return null;
//        //}



//        //[Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
//        //public void CombineFiles(List<FileInfo> fileList, FileInfo toFile)
//        //{
//        //    Core.CombineFiles(fileList, toFile, (msg) => _logger.LogInformation(msg));
//        //}

//        //[Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
//        //public Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile)
//        //{
//        //    return Core.CombineFilesAsync(fileList, toFile);
//        //}
//        //public Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
//        //{
//        //    return Core.CombineFilesAsync(fileList, toDirectory, fileName, (msg) => _logger.LogInformation(msg));
//        //}
//        //public FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
//        //{
//        //    return Core.CombineFiles(fileList, toDirectory, fileName, (msg) => _logger.LogInformation(msg));
//        //}
//    }
//}
