using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.PdfHandling
{
    public static class Core
    {

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
