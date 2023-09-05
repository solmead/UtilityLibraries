using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileExtensions;

namespace Utilities.PdfHandling
{
    public static class Core
    {

        public static PdfConfig config = null;

        public static FileInfo CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName, Action<string> log)
        {

            if (Core.config == null)
            {
                throw new Exception("Please call services.InitilizePdfHandling()");
            }

            if (log == null)
            {
                log = (msg) => { };
            }

            var bhbind = new BasicHttpBinding();// BasicHttpSecurityMode.Transport);
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
            EndpointAddress endpointAddress = new EndpointAddress(Core.config.ConnectionString);
            var ps = new PdfService.PdfConvertClient(bhbind, endpointAddress);

            //var ps = new PdfService.PdfConvertClient(new PdfService.PdfConvertClient.EndpointConfiguration(), Core.config.ConnectionString);

            var fileData = new List<PdfService.FileItem>();

            log(fileList.Count + " files passed in.");


            fileList.ForEach((file) =>
            {
                file.Refresh();
                if (file.Exists)
                {
                    log("File [" + file.FullName + "] Exists");
                    fileData.Add(new PdfService.FileItem()
                    {
                        FileName = file.Name.Replace(",", "_").Replace(" ", "_"),
                        Data = File.ReadAllBytes(file.FullName)
                    });
                } else
                {
                    log("File [" + file.FullName + "] Missing");
                }
            });

            log(fileData.Count + " files existed.");

            foreach (var fd in fileData)
            {
                Debug.WriteLine(fd.FileName + " " + fd.Data.Length);
            }

            FileInfo toFile = new FileInfo(toDirectory.FullName + "/" + fileName);

            try
            {
                log("Calling PdfService");
                var finalFile = ps.CombineFilesIntoOnePdf(fileData);
                log("PdfService Returned: [" + finalFile?.FileName +"] - Size: [" + finalFile?.Data?.Length + "]");

                var fFile = new FileInfo(toDirectory.FullName + "/" + finalFile.FileName);

                toFile = new FileInfo(toDirectory.FullName + "/" + toFile.FileNameWithoutExtension() + fFile.Extension);

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

        [Obsolete("Use CombineFiles(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        public static void CombineFiles(List<FileInfo> fileList, FileInfo toFile, Action<string> log)
        {
            try
            {

                toFile.Refresh();

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

                var fFile = CombineFiles(fileList, toFile.Directory, toFile.FileNameWithoutExtension(), log);



            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static async Task<FileInfo> CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)
        {
            if (Core.config == null)
            {
                throw new Exception("Please call services.InitilizePdfHandling()");
            }

            var bhbind = new BasicHttpBinding();// BasicHttpSecurityMode.Transport);
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
            EndpointAddress endpointAddress = new EndpointAddress(Core.config.ConnectionString);
            var ps = new PdfService.PdfConvertClient(bhbind, endpointAddress);

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

            FileInfo toFile = null;

            try
            {
                var finalFile = await ps.CombineFilesIntoOnePdfAsync(fileData);

                var fFile = new FileInfo(toDirectory.FullName + "/" + finalFile.FileName);

                toFile = new FileInfo(toDirectory.FullName + "/" + fileName + fFile.Extension);

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
        [Obsolete("Use CombineFilesAsync(List<FileInfo> fileList, DirectoryInfo toDirectory, string fileName)", true)]
        public static async Task CombineFilesAsync(List<FileInfo> fileList, FileInfo toFile)
        {
            try
            {

                toFile.Refresh();

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

                var fFile = await CombineFilesAsync(fileList, toFile.Directory, toFile.FileNameWithoutExtension());



            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
