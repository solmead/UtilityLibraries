using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Utilities.PdfHandling.NetCore
{
    [Obsolete("No longer used, now setup when Configurator called", true)]
    public class PdfCreator : IPdfCreation
    {

        private static string HiQPDFSerial { get; set; } = "DUVkXF1p-a0Fkb39s-f3Q8PSM9-LTwtPy01-PDUtPjwj-PD8jNDQ0-NA==";

        private readonly ILogger _logger;
        public PdfCreator(ILogger logger)
        {
            _logger = logger;
        }

        [Obsolete("No longer Supported", true)]
        public void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        {
            throw new Exception("No longer Supported");
        }

        public void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        {
            //Log("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
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
            //Log("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
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
        public byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreator.GetPdfFromUrl calling temp webservice");


                var bhbind = new BasicHttpsBinding();// BasicHttpSecurityMode.Transport);
                bhbind.MaxBufferSize = int.MaxValue;
                bhbind.MaxReceivedMessageSize = int.MaxValue;
                bhbind.OpenTimeout = new TimeSpan(12, 0, 0);
                bhbind.ReceiveTimeout = new TimeSpan(12, 0, 0);
                bhbind.SendTimeout = new TimeSpan(12, 0, 0);
                bhbind.CloseTimeout = new TimeSpan(12, 0, 0);
                bhbind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                bhbind.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
                bhbind.ReaderQuotas.MaxDepth = int.MaxValue;
                bhbind.ReaderQuotas.MaxArrayLength = int.MaxValue;
                bhbind.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                EndpointAddress endpointAddress = new EndpointAddress("https://webservices-ext.uc.edu/nightride/PdfCreate.svc");
                using (var us = new PdfCreateService.PdfCreateServiceClient(bhbind, endpointAddress))
                {
                    us.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                    var arr = us.GetPdfFromUrl(url, (PdfCreateService.PageOrientation)orientation);

                    _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreator.GetPdfFromUrl temp webservice returned [" + arr.Length + "] bytes");
                    return arr;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Utilities.PdfHandling.NetCore.PdfCreator.GetPdfFromUrl Error -> " +  ex.ToString());
            } 
            return null;
        }
        public async Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreator.GetPdfFromUrlAsync calling temp webservice");

                var bhbind = new BasicHttpsBinding();// BasicHttpSecurityMode.Transport);
                bhbind.MaxBufferSize = int.MaxValue;
                bhbind.MaxReceivedMessageSize = int.MaxValue;
                bhbind.OpenTimeout = new TimeSpan(12, 0, 0);
                bhbind.ReceiveTimeout = new TimeSpan(12, 0, 0);
                bhbind.SendTimeout = new TimeSpan(12, 0, 0);
                bhbind.CloseTimeout = new TimeSpan(12, 0, 0);
                bhbind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                bhbind.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
                bhbind.ReaderQuotas.MaxDepth = int.MaxValue;
                bhbind.ReaderQuotas.MaxArrayLength = int.MaxValue;
                bhbind.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                EndpointAddress endpointAddress = new EndpointAddress("https://webservices-ext.uc.edu/nightride/PdfCreate.svc");
                using (var us = new PdfCreateService.PdfCreateServiceClient(bhbind, endpointAddress))
                {
                    us.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                    var arr = await us.GetPdfFromUrlAsync(url, (PdfCreateService.PageOrientation)orientation);
                    _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreator.GetPdfFromUrlAsync temp webservice returned [" + arr.Length + "] bytes");
                    return arr;
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Utilities.PdfHandling.NetCore.PdfCreator.GetPdfFromUrl Error -> " + ex.ToString());
            }
            return null;
        }



        [Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)")]
        public void CombineFiles(List<FileInfo> fileList, FileInfo toFile)
        {
            Core.CombineFiles(fileList, toFile, (msg)=>_logger.LogInformation(msg));
        }

        [Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)")]
        public Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile)
        {
            return Core.CombineFilesAsync(fileList, toFile);
        }
        public Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            return Core.CombineFilesAsync(fileList, toDirectory, fileName);
        }
        public FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            return Core.CombineFiles(fileList, toDirectory, fileName, (msg) => _logger.LogInformation(msg));
        }

    }
}
