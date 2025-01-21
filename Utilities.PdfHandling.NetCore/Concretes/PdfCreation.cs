using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UC.PdfServices.Client.Abstracts;
using UC.PdfServices.Client.Models;
using Utilities.FileExtensions;
using static System.Net.WebRequestMethods;

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

        public async Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreation.GetPdfFromUrlAsync calling new client");


                var orient = (UC.PdfServices.Client.Abstracts.PageOrientation)orientation;
                var resp = await _pdfServicesClient.GetPdfFromUrl(url, orient);

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


        public async Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            try
            {
                _logger.LogInformation("Utilities.PdfHandling.NetCore.PdfCreation.GetPdfFromUrlAsync calling new client");

                var fileData = new List<FileEntry>();

                fileList.ForEach((file) =>
                {
                    file.Refresh();
                    if (file.Exists)
                    {
                        fileData.Add(new FileEntry()
                        {
                            FileName = file.Name.Replace(",", "_").Replace(" ", "_"),
                            Data = System.IO.File.ReadAllBytes(file.FullName)
                        });
                    }
                });

                var fList = new FileList
                {
                    Files = fileData
                };


                var resp = await _pdfServicesClient.CombineFilesIntoOnePdf(fList);

                if (resp.IsSuccess)
                {
                    var finalFile = resp.ValueOrDefault;



                    var fFile = new FileInfo(toDirectory.FullName + "/" + finalFile.FileName);
                    var initFile = new FileInfo(toDirectory.FullName + "/" + fileName);


                    var toFile = new FileInfo(toDirectory.FullName + "/" + initFile.FileNameWithoutExtension() + fFile.Extension);

                    var f = new FileStream(toFile.FullName, FileMode.Create);
                    f.Write(finalFile.Data, 0, finalFile.Data.Length);
                    f.Close();
                    toFile.Refresh();


                    return toFile;
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
    }
}
