using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiQPdf;

namespace Utilities.PdfHandling
{
    public static class Core
    {
        public enum PageOrientation
        {
            Portrait,
            Landscape
        }

        public static string HiQPDFSerial { get; set; } = "DUVkXF1p-a0Fkb39s-f3Q8PSM9-LTwtPy01-PDUtPjwj-PD8jNDQ0-NA==";


        public static void SavePdfFromHtml(string html, string baseUrl, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
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
            var data = GetPdfFromHtml(html, baseUrl, orientation);
            if (data != null)
            {
                var st = file.OpenWrite();
                st.Write(data, 0, data.Length);
                st.Close();
            }
            file.Refresh();


            //Log("SavePdfFromUrl End");
        }

        public static  void SavePdfFromUrl(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
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

        public static async Task SavePdfFromUrlAsync(string url, FileInfo file, PageOrientation orientation = PageOrientation.Portrait)
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

        public static Byte[] GetPdfFromHtml(string html, string baseUrl, PageOrientation orientation = PageOrientation.Portrait)
        {


            var htmlToPdfConverter = new HtmlToPdf()
            {
                SerialNumber = HiQPDFSerial,
                BrowserWidth = 1300,
                //LayoutWithHinting = true,
                TriggerMode = ConversionTriggerMode.WaitTime,
                WaitBeforeConvert = 5,
                HtmlLoadedTimeout = 2400
            };
            htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
            if ((orientation == PageOrientation.Landscape))
            {
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;
            }

            Byte[] pdfBuffer;

            pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(html, baseUrl);

            return pdfBuffer;
        }
        public static byte[] GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            //Log("GetPdfFromUrl Start Url:" + url);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            byte[] pdfBuffer = null;

            if ((url.Contains("?")))
            {
                url = url + "&DisplayPdfOn=true";
            }
            else
            {
                url = url + "?DisplayPdfOn=true";
            }


            var htmlToPdfConverter = new HtmlToPdf()
            {
                SerialNumber = HiQPDFSerial,
                BrowserWidth = 1300,
                //LayoutWithHinting = true,
                TriggerMode = ConversionTriggerMode.WaitTime,
                WaitBeforeConvert = 5,
                HtmlLoadedTimeout = 2400
            };
            htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
            if ((orientation == PageOrientation.Landscape))
            {
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;
            }


            //Log("htmlToPdfConverter.ConvertUrlToMemory Before");
            try
            {
                pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);
            }
            catch (Exception e)
            {
                //e.LogToElmah();
                throw e;
            }
            //Log("htmlToPdfConverter.ConvertUrlToMemory After");


            //Log("GetPdfFromUrl End");
            return pdfBuffer;
        }
        public static Task<byte[]> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            //Log("GetPdfFromUrl Start Url:" + url);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            byte[] pdfBuffer = null;

            if ((url.Contains("?")))
            {
                url = url + "&DisplayPdfOn=true";
            }
            else
            {
                url = url + "?DisplayPdfOn=true";
            }


            var htmlToPdfConverter = new HtmlToPdf()
            {
                SerialNumber = HiQPDFSerial,
                BrowserWidth = 1300,
                //LayoutWithHinting = true,
                TriggerMode = ConversionTriggerMode.WaitTime,
                WaitBeforeConvert = 5,
                HtmlLoadedTimeout = 2400
            };
            htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
            htmlToPdfConverter.Document.Margins = new PdfMargins(5);
            htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
            if ((orientation == PageOrientation.Landscape))
            {
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Landscape;
            }


            //Log("htmlToPdfConverter.ConvertUrlToMemory Before");
            try
            {
                pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);
            }
            catch (Exception e)
            {
                //e.LogToElmah();
                throw e;
            }
            //Log("htmlToPdfConverter.ConvertUrlToMemory After");


            //Log("GetPdfFromUrl End");
            return Task.FromResult(pdfBuffer);
        }



        public static void CombineFiles(List<FileInfo> fileList, FileInfo toFile)
        {
            try
            {

                toFile.Refresh();
                var ps = new PdfService.PdfConvertClient();

                var fileData = new List<PdfService.FileItem>();

                fileList.ForEach((file) =>
                {
                    file.Refresh();
                    if (file.Exists)
                    {
                        fileData.Add(new PdfService.FileItem()
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

                try
                {
                    var finalFile = ps.CombineFilesIntoOnePdf(fileData);

                    if (toFile.Exists)
                    {
                        try
                        {
                            toFile.Delete();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    var f = new FileStream(toFile.FullName, FileMode.Create);
                    f.Write(finalFile.Data, 0, finalFile.Data.Length);
                    f.Close();
                    toFile.Refresh();
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not combine files: " + ex.Message, ex);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static async Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile)
        {
            try
            {

                toFile.Refresh();
                var ps = new PdfService.PdfConvertClient();

                var fileData = new List<PdfService.FileItem>();

                fileList.ForEach((file) =>
                {
                    file.Refresh();
                    if (file.Exists)
                    {
                        fileData.Add(new PdfService.FileItem()
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

                try
                {
                    var finalFile = await ps.CombineFilesIntoOnePdfAsync(fileData);

                    if (toFile.Exists)
                    {
                        try
                        {
                            toFile.Delete();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    var f = new FileStream(toFile.FullName, FileMode.Create);
                    f.Write(finalFile.Data, 0, finalFile.Data.Length);
                    f.Close();
                    toFile.Refresh();
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not combine files: " + ex.Message, ex);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }









    }
}
