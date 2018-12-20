using System;
using System.Collections.Generic;
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
                SerialNumber = "MnpbY2JW-VH5bUEBT-QEsEHAIS-AxIAEgoH-BRIBAxwD-ABwLCwsL",
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
            }
            //Log("htmlToPdfConverter.ConvertUrlToMemory After");


            //Log("GetPdfFromUrl End");
            return pdfBuffer;
        }


    }
}
