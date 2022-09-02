using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utilities.FileExtensions
{
    public static class Extensions
    {

        public static List<string> VideoList = new List<string>() { "FLV", "MOD", "AVI", "MPG", "MPEG", "MOV", "WMV", "VOB", "VRO", "MTS", "QT", "SWF", "MP4", "M4V", "OGV" };
        public static List<string> ImageList = new List<string>() { "JPG", "JPEG", "PNG", "BMP", "TGA", "GIF", "ICO" };
        public static List<string> AudioList = new List<string>() { "MP3", "WAV", "OGG", "AAC", "WMA", "M4A" };


        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };




        public static string SizeSuffix(this int value, int decimalPlaces = 1)
        {
            return ((long)value).SizeSuffix(decimalPlaces);
        }
        public static string SizeSuffix(this long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + (-value).SizeSuffix(); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << mag * 10);

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }



        /// <summary>
        /// Removes the illegal characters from a file name.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>A file name with all illegal characters removed.</returns>
        public static string RemoveIllegalCharacters(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "";
            }
            return RemoveIllegalCharactersFromString(fileName).ToString();
        }

        /// <summary>
        /// Removes the illegal characters from a string.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>A SringBuilder containing a safe file name.</returns>
        private static StringBuilder RemoveIllegalCharactersFromString(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "";
            }
            StringBuilder result = new StringBuilder(fileName.Trim());
            if (!string.IsNullOrEmpty(fileName))
            {
                char[] invalidFileNameCharacters =
                    ("\"#%&*/:<>?\\{|}~'".ToCharArray()).Union(Path.GetInvalidFileNameChars()).Union(
                        Path.GetInvalidPathChars()).Union(new[] { Path.PathSeparator }).ToArray();

                foreach (char invalidFileNameCharacter in invalidFileNameCharacters)
                {
                    result.Replace(invalidFileNameCharacter.ToString(), string.Empty);
                }
            }

            return result;
        }

        public static FileInfo GetFileInfo(DirectoryInfo directory, string filename)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }

            var fi = new FileInfo(directory.FullName + "\\" + filename);
            var bFi = fi;
            if (fi.Exists)
            {
                var cnt = 2;
                fi = new FileInfo(directory.FullName + "\\" + bFi.FileNameWithoutExtension() + "_" + cnt + bFi.Extension);
                while (fi.Exists)
                {
                    cnt++;
                    fi = new FileInfo(directory.FullName + "\\" + bFi.FileNameWithoutExtension() + "_" + cnt + bFi.Extension);
                    fi.Refresh();
                }
            }
            fi.Refresh();

            return fi;
        }
        /// <summary>
        /// Removes the illegal file name characters from a string.
        /// </summary>
        /// <param name="fileName">The original string value.</param>
        /// <returns>A string with all illegal filename characters removed.</returns>
        public static string RemoveIllegalFileNameCharacters(this string fileName)
        {
            return RemoveIllegalCharactersFromString(fileName).ToString();
        }

        /// <summary>
        /// Generates a unique file name with all illegal characters removed.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A safe and unique file name.</returns>
        public static string GenerateUniqueName(this FileInfo file)
        {
            return
                (Path.GetFileNameWithoutExtension(file.Name) + "_" + DateTime.UtcNow.Ticks +
                 Path.GetExtension(file.Name)).RemoveIllegalFileNameCharacters();
        }
        public static FileInfo MoveFile(FileInfo file, DirectoryInfo directory, string filename)
        {
            var fi = GetFileInfo(directory, filename);
            file.MoveTo(fi.FullName);
            fi.Refresh();
            return fi;
        }
        public static void WriteToFile(this FileInfo fi, MemoryStream ms)
        {
            var s = fi.OpenWrite();
            s.Write(ms.ToArray(), 0, (int)ms.Length);
            s.Close();

        }
        public static string FileNameWithoutExtension(this FileInfo File)
        {
            return File.Name.Substring(0, File.Name.Length - File.Extension.Length);
        }
        public static bool IsImage(this FileInfo file)
        {
            if (file.Extension.Length<2)
            {
                return false;
            }
            var ext = file.Extension.Substring(1);
            return ImageList.Contains(ext.ToUpper());
        }
        public static bool IsVideo(this FileInfo file)
        {
            if (file.Extension.Length < 2)
            {
                return false;
            }
            var ext = file.Extension.Substring(1);
            return VideoList.Contains(ext.ToUpper());
        }
        public static bool IsAudio(this FileInfo file)
        {
            if (file.Extension.Length < 2)
            {
                return false;
            }
            var ext = file.Extension.Substring(1);
            return AudioList.Contains(ext.ToUpper());
        }
        //public static string ContentType(this System.IO.FileInfo File)
        //{
        //    string Extension = File.Extension.ToUpper().Substring(1);


        //    var mContentType = "application/octet-stream";
        //    if (VideoList.Contains(Extension))
        //        mContentType = "video/mpeg";
        //    switch (Extension)
        //    {
        //        case "JPG":
        //            {
        //                mContentType = "image/jpeg";
        //                break;
        //            }

        //        case "GIF":
        //            {
        //                mContentType = "image/gif";
        //                break;
        //            }

        //        case "PNG":
        //            {
        //                mContentType = "image/png";
        //                break;
        //            }

        //        case "ICO":
        //            {
        //                mContentType = "image/ico";
        //                break;
        //            }

        //        case "BMP":
        //            {
        //                mContentType = "image/bmp";
        //                break;
        //            }

        //        case "MP3":
        //            {
        //                mContentType = "audio/mp3";
        //                break;
        //            }

        //        case "WAV":
        //            {
        //                mContentType = "audio/wav";
        //                break;
        //            }

        //        case "M4A":
        //            {
        //                mContentType = "audio/x-m4a";
        //                break;
        //            }

        //        case "MP4":
        //            {
        //                mContentType = "video/mp4";
        //                break;
        //            }

        //        case "M4V":
        //            {
        //                mContentType = "video/x-m4v";
        //                break;
        //            }

        //        case "MOV":
        //            {
        //                mContentType = "video/quicktime";
        //                break;
        //            }

        //        case "PDF":
        //            {
        //                mContentType = "application/pdf";
        //                break;
        //            }

        //        case "OGV":
        //            {
        //                mContentType = "video/ogg";
        //                break;
        //            }

        //        case "WEBM":
        //            {
        //                mContentType = "video/webm";
        //                break;
        //            }
        //    }

        //    return mContentType;
        //}


    }
}
