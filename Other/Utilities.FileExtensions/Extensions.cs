using System;
using System.Collections.Generic;
using System.IO;

namespace Utilities.FileExtensions
{
    public static class Extensions
    {

        public static List<string> VideoList = new List<string>() { "FLV", "MOD", "AVI", "MPG", "MPEG", "MOV", "WMV", "VOB", "VRO", "MTS", "QT", "SWF", "MP4", "M4V", "OGV" };
        public static List<string> ImageList = new List<string>() { "JPG", "JPEG", "PNG", "BMP", "TGA", "GIF", "ICO" };
        public static List<string> AudioList = new List<string>() { "MP3", "WAV", "OGG", "AAC", "WMA", "M4A" };

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
            var ext = file.Extension.Substring(1);
            return ImageList.Contains(ext.ToUpper());
        }
        public static bool IsVideo(this FileInfo file)
        {
            var ext = file.Extension.Substring(1);
            return VideoList.Contains(ext.ToUpper());
        }
        public static bool IsAudio(this FileInfo file)
        {
            var ext = file.Extension.Substring(1);
            return AudioList.Contains(ext.ToUpper());
        }
        public static string ContentType(this System.IO.FileInfo File)
        {
            string Extension = File.Extension.ToUpper().Substring(1);


            var mContentType = "application/octet-stream";
            if (VideoList.Contains(Extension))
                mContentType = "video/mpeg";
            switch (Extension)
            {
                case "JPG":
                    {
                        mContentType = "image/jpeg";
                        break;
                    }

                case "GIF":
                    {
                        mContentType = "image/gif";
                        break;
                    }

                case "PNG":
                    {
                        mContentType = "image/png";
                        break;
                    }

                case "ICO":
                    {
                        mContentType = "image/ico";
                        break;
                    }

                case "BMP":
                    {
                        mContentType = "image/bmp";
                        break;
                    }

                case "MP3":
                    {
                        mContentType = "audio/mp3";
                        break;
                    }

                case "WAV":
                    {
                        mContentType = "audio/wav";
                        break;
                    }

                case "M4A":
                    {
                        mContentType = "audio/x-m4a";
                        break;
                    }

                case "MP4":
                    {
                        mContentType = "video/mp4";
                        break;
                    }

                case "M4V":
                    {
                        mContentType = "video/x-m4v";
                        break;
                    }

                case "MOV":
                    {
                        mContentType = "video/quicktime";
                        break;
                    }

                case "PDF":
                    {
                        mContentType = "application/pdf";
                        break;
                    }

                case "OGV":
                    {
                        mContentType = "video/ogg";
                        break;
                    }

                case "WEBM":
                    {
                        mContentType = "video/webm";
                        break;
                    }
            }

            return mContentType;
        }


    }
}
