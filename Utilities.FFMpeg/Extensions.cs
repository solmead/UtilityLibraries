using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.EnumExtensions;
using Utilities.FileExtensions;

namespace Utilities.MediaConverter
{
    public static class FFMpegExtensions
    {

        public static List<string> VideoList = new List<string> { "FLV", "MOD", "AVI", "MPG", "MPEG", "MOV", "WMV", "VOB", "VRO", "MTS", "QT", "SWF", "MP4", "M4V" };
        public static List<string> ImageList = new List<string> { "JPG", "JPEG", "PNG", "BMP", "TGA", "GIF" };
        public static List<string> AudioList = new List<string> { "MP3", "WAV", "OGG", "AAC", "WMA", "M4A" };



        internal static string AudioFormat(this AudioEncodingEnum AudioEncoding)
        {
            switch (AudioEncoding)
            {
                case AudioEncodingEnum.mp3:
                        return "libmp3lame";

                case AudioEncodingEnum.mp2:
                        return "mp2";

                case AudioEncodingEnum.ac3:
                        return "ac3";

                case AudioEncodingEnum.aac:
                        return "libfaac";

                default:
                        return "libmp3lame";
            }
        }
        internal static string VideoFormat(this VideoEncodingEnum VideoEncoding)
        {
            switch (VideoEncoding)
            {
                case VideoEncodingEnum.h264:
                        return "libx264";

                default:
                        return VideoEncoding.ToString();
            }
        }

        internal static void Refresh(this VideoInfo video)
        {
            var tsk = Task.Run(async () =>
            {
                await RefreshAsync(video);
            });


            tsk.Wait();
            var a = 1;
        }
        internal static async Task RefreshAsync(this VideoInfo video)
        {
            var ffmpeg = new FFMpeg(Core.Logger);
            await ffmpeg.RefreshAsync(video);
            var a = 1;
        }

        //public string FFMpegLocation
        //{
        //    get
        //    {
        //        if (FFMpegBaseDirectory == "")
        //            FFMpegBaseDirectory = My.Application.Info.DirectoryPath + @"\ffmpeg\";
        //        return FFMpegBaseDirectory;
        //    }
        //}
        //public string FLVToolLocation
        //{
        //    get
        //    {
        //        return FFMpegBaseDirectory;
        //    }
        //}
        internal static string GetValueOnLine(this string Line, string Name)
        {
            var pos = Line.ToUpper().IndexOf(Name.ToUpper() + "=");
            if ((pos >= 0))
            {
                var Part = Line.Substring(pos + Name.Length + 1);
                Part = Part.Trim();
                var EndPos = Part.IndexOf(" ");
                if (EndPos < 0)
                    EndPos = Part.Length;
                var Value = Part.Substring(0, EndPos - 1);
                return Value;
            }
            return "";
        }

        internal static string SafeGet(this string[] Entries, int Pos)
        {
            if (Pos >= Entries.Length)
                return "";
            else if (Pos < 0)
                return "";
            else
                return Entries[Pos];
        }
        internal static Double Val(string value)
        {
            String result = String.Empty;
            foreach (char c in value)
            {
                if (Char.IsNumber(c) || (c.Equals('.') && result.Count(x => x.Equals('.')) == 0))
                    result += c;
                else if (!c.Equals(' '))
                    return String.IsNullOrEmpty(result) ? 0 : Convert.ToDouble(result);
            }
            return String.IsNullOrEmpty(result) ? 0 : Convert.ToDouble(result);
        }

        internal static double AsDouble(this string number)
        {
            number = number.Trim();
            //return (double.TryParse(number, out var result) ? result : 0);
            return Val(number);
        }
        internal static int AsInt(this string number)
        {
            number = number.Trim();
            //return (int.TryParse(number, out var result) ? result : 0);
            return (int)Val(number);
        }



    }
}
