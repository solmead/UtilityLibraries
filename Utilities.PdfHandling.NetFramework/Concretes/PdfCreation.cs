using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PDFWebService;
using Utilities.FileExtensions;
using Utilities.PdfHandling.NetFramework.Configuration;

namespace Utilities.PdfHandling.NetFramework.Concretes
{
    public class PdfCreation : IPdfCreation
    {

        private readonly ILogger _logger;
        public PdfCreation(ILogger logger)
        {
            _logger = logger;
        }

        private string GetEndPointUrl()
        {
            if (Configurator.config.CurrentServer == ServerEnum.Development)
            {
                return "https://webservices-webdev2.uc.edu/pdfservices/PDFHandling.svc";
            }
            if (Configurator.config.CurrentServer == ServerEnum.QA)
            {
                return "https://webservices-webqa2.uc.edu/pdfservices/PDFHandling.svc";
            }
            if (Configurator.config.CurrentServer == ServerEnum.Scan)
            {
                return "https://webservices-scan2.uc.edu/pdfservices/PDFHandling.svc";
            }
            if (Configurator.config.CurrentServer == ServerEnum.Production)
            {
                return "https://webservices-ext.uc.edu/pdfservices/PDFHandling.svc";
            }


            return "https://webservices-webdev2.uc.edu/pdfservices/PDFHandling.svc";
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


        //public void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        //{
        //    //Log("SavePdfFromUrl Start Url:" + url + " file:" + file.FullName);
        //    if (!file.Directory.Exists)
        //    {
        //        file.Directory.Create();
        //    }
        //    if (file.Exists)
        //    {
        //        file.Delete();
        //    }
        //    var data = GetPdfFromUrl(url, orientation);
        //    if (data != null)
        //    {
        //        var st = file.OpenWrite();
        //        st.Write(data, 0, data.Length);
        //        st.Close();
        //    }
        //    file.Refresh();


        //    //Log("SavePdfFromUrl End");
        //}
        
        //public byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrl calling webservice");


        //        var bhbind = new BasicHttpsBinding();// BasicHttpSecurityMode.Transport);
        //        bhbind.MaxBufferSize = int.MaxValue;
        //        bhbind.MaxReceivedMessageSize = int.MaxValue;
        //        bhbind.OpenTimeout = new TimeSpan(12, 0, 0);
        //        bhbind.ReceiveTimeout = new TimeSpan(12, 0, 0);
        //        bhbind.SendTimeout = new TimeSpan(12, 0, 0);
        //        bhbind.CloseTimeout = new TimeSpan(12, 0, 0);
        //        bhbind.ReaderQuotas.MaxStringContentLength = int.MaxValue;
        //        bhbind.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
        //        bhbind.ReaderQuotas.MaxDepth = int.MaxValue;
        //        bhbind.ReaderQuotas.MaxArrayLength = int.MaxValue;
        //        bhbind.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
        //        var endUrl = GetEndPointUrl();
        //        _logger.LogInformation("Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrl call to [" + endUrl + "]");
        //        EndpointAddress endpointAddress = new EndpointAddress(endUrl);
        //        using (var us = new PDFWebService.PdfServiceClient(bhbind, endpointAddress))
        //        {
        //            us.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);


        //            _logger.LogInformation("Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrl calling service");
        //            var arr = us.GetPdfFromUrl(url, (PDFWebService.PageOrientation)orientation);

        //            _logger.LogInformation("Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrl webservice returned [" + arr.Data.Length + "] bytes");
        //            return arr.Data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrl Error -> " + ex.ToString());
        //    }
        //    return null;
        //}
        public async Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrlAsync calling webservice");

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
                EndpointAddress endpointAddress = new EndpointAddress(GetEndPointUrl());
                using (var us = new PDFWebService.PdfServiceClient(bhbind, endpointAddress))
                {
                    us.InnerChannel.OperationTimeout = new TimeSpan(0, 10, 0);
                    var arr = await us.GetPdfFromUrlAsync(url, (PDFWebService.PageOrientation)orientation);
                    _logger.LogInformation("Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrlAsync webservice returned [" + arr.Data.Length + "] bytes");
                    return arr.Data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Utilities.PdfHandling.NetFramework.PdfCreation.GetPdfFromUrl Error -> " + ex.ToString());
            }
            return null;
        }

        public async Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            var bhbind = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
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
            EndpointAddress endpointAddress = new EndpointAddress(GetEndPointUrl());
            var ps = new PDFWebService.PdfServiceClient(bhbind, endpointAddress);

            var fileData = new List<FileItem>();

            fileList.ForEach((file) =>
            {
                file.Refresh();
                if (file.Exists)
                {
                    fileData.Add(new FileItem()
                    {
                        FileName = file.Name.Replace(",", "_").Replace(" ", "_"),
                        Data = File.ReadAllBytes(file.FullName)
                    });
                }
            });


            foreach (var fd in fileData)
            {
                Debug.WriteLine(fd.FileName + " " + fd.Data.Length);
            }

            FileInfo toFile = null;

            try
            {
                var finalFile = await ps.CombineFilesIntoOnePdfAsync(fileData);

                var fFile = new FileInfo(toDirectory.FullName + "/" + finalFile.FileName);
                var initFile = new FileInfo(toDirectory.FullName + "/" + fileName);


                toFile = new FileInfo(toDirectory.FullName + "/" + initFile.FileNameWithoutExtension() + fFile.Extension);

                var f = new FileStream(toFile.FullName, FileMode.Create);
                f.Write(finalFile.Data, 0, finalFile.Data.Length);
                f.Close();
                toFile.Refresh();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not combine files: " + ex.Message, ex);
            }

            return toFile;
        }



        //[Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        //public void CombineFiles(List<FileInfo> fileList, FileInfo toFile)
        //{
        //    throw new NotImplementedException();
        //}

        //[Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        //public Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile)
        //{
        //    throw new NotImplementedException();
        //}
        //public Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        //{
        //    return Core.CombineFilesAsync(fileList, toDirectory, fileName, (msg) => _logger.LogInformation(msg));
        //}
        //public FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        //{
        //    throw new NotImplementedException();
        //}

        //public void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
        //{
        //    throw new NotImplementedException();
        //}

        //public byte[] GetPdfFromHtml(string html, string baseUrl, PageOrientation orientation = PageOrientation.Portrait)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
