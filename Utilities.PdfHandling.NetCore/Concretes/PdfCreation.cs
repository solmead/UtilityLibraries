using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.PdfHandling.NetCore.Abstracts;

namespace Utilities.PdfHandling.NetCore.Concretes
{
    public class PdfCreation : IPdfCreation
    {

        private readonly ILogger _logger;
        private readonly IPdfServicesClient _pdfServicesClient;

        public PdfCreation(ILogger logger, IPdfServicesClient pdfServicesClient)
        {
            _logger = logger;
            _pdfServicesClient = pdfServicesClient;
        }

        [Obsolete("No longer Supported", true)]
        public void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        {
            throw new Exception("No longer Supported");
        }

        public void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        {
            _logger.LogInformation("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            if (file.Exists)
            {
                file.Delete();
            }
            var data = GetPdfFromUrl(url, orientation);
            if (data != null)
            {
                var st = file.OpenWrite();
                st.Write(data, 0, data.Length);
                st.Close();
            }
            file.Refresh();


            //Log("SavePdfFromUrl End");
        }

        public async Task SavePdfFromUrlAsync(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        {
            _logger.LogInformation("SavePdfFromUrlAsync Start Url:" + url + " file:" + file.FullName);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            if (file.Exists)
            {
                file.Delete();
            }
            var data = await GetPdfFromUrlAsync(url, orientation);
            if (data != null)
            {
                var st = file.OpenWrite();
                st.Write(data, 0, data.Length);
                st.Close();
            }
            file.Refresh();


            //Log("SavePdfFromUrl End");
        }

        [Obsolete("No longer Supported", true)]
        public byte[] GetPdfFromHtml(string html, string baseUrl, PageOrientation orientation = PageOrientation.Portrait)
        {
            throw new Exception("No longer Supported");
        }
        [Obsolete("Switch to using Async version of calls -> GetPdfFromUrlAsync")]
        public byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.Concretes.PdfCreation.GetPdfFromUrl calling temp webservice");


                var resp = _pdfServicesClient.GetPdfFromUrl(url, orientation);

                if (resp.IsSuccess)
                {
                    var dt = resp.ValueOrDefault;
                    return dt.Data;
                }

                _logger.LogError("Utilities.PdfHandling.Concretes.PdfCreation.GetPdfFromUrl Error -> " + resp.Errors.FirstOrDefault()?.Message);

                return null;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Utilities.PdfHandling.Concretes.PdfCreation.GetPdfFromUrl Error -> " + ex.ToString());
            }
            return null;
        }
        public async Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreation.GetPdfFromUrlAsync calling new client");


                var resp = await _pdfServicesClient.GetPdfFromUrlAsync(url, orientation);

                if (resp.IsSuccess)
                {
                    var dt = resp.ValueOrDefault;
                    return dt.Data;
                }

                _logger.LogError("Utilities.PdfHandling.Concretes.PdfCreation.GetPdfFromUrl Error -> " + resp.Errors.FirstOrDefault()?.Message);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Utilities.PdfHandling.Concretes.PdfCreation.GetPdfFromUrl Error -> " + ex.ToString());
            }
            return null;
        }



        [Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        public void CombineFiles(List<FileInfo> fileList, FileInfo toFile)
        {
            Core.CombineFiles(fileList, toFile, (msg) => _logger.LogInformation(msg));
        }

        [Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        public Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile)
        {
            return Core.CombineFilesAsync(fileList, toFile);
        }
        public Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            return Core.CombineFilesAsync(fileList, toDirectory, fileName, (msg) => _logger.LogInformation(msg));
        }
        public FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            throw new Exception("No longer Supported");
        }
    }
}
