using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utilities.MediaConverter
{
    public class VideoInfo
    {


        public System.IO.FileInfo File { get; set; }
        public int Width { get; set; } = 600;
        public int Height { get; set; } = 300;
        public int BitRate { get; set; } = 1000;

        public SoundTypeEnum Channels { get; set; } = SoundTypeEnum.Stereo;

        public int AudioBitRate { get; set; } = 128;

        public double Duration { get; set; } = 0;
        public bool HasVideo { get; set; } = false;
        public bool HasAudio { get; set; } = false;

        public VideoEncodingEnum VideoEncoding { get; set; } = VideoEncodingEnum.flv;
        public VideoSubEncodingEnum VideoSubEncoding { get; set; } = VideoSubEncodingEnum.yuv420p;
        public AudioEncodingEnum AudioEncoding { get; set; } = AudioEncodingEnum.None;
        public int AudioFrequency { get; set; } = 44100;

        public double FrameRate { get; set; } = 24; // 29.97
        public string PAR = "1:1";
        public string DAR = "16:9";


        public double PAR_Value
        {
            get
            {
                try
                {
                    string[] Str = PAR.Split(':');

                    double v1 = 0;
                    double v2 = 0;
                    double.TryParse(Str[0], out v1);
                    double.TryParse(Str[1], out v2);
                    if (v2==0)
                    {
                        v2 = 1;
                    }


                    return v1 / v2;
                }
                catch (Exception ex)
                {
                }
                return 1;
            }
        }
        public double DAR_Value
        {
            get
            {
                try
                {
                    string[] Str = DAR.Split(':');

                    double v1 = 0;
                    double v2 = 0;
                    double.TryParse(Str[0], out v1);
                    double.TryParse(Str[1], out v2);
                    if (v2 == 0)
                    {
                        v2 = 1;
                    }


                    return v1 / v2;
                }
                catch (Exception ex)
                {
                }
                return 16 / (double)9;
            }
        }
        public int WidthAdjusted
        {
            get
            {
                if (PAR_Value <= 1)
                    return (int)(Width * PAR_Value);
                else
                    return Width;
            }
        }
        public int HeightAdjusted
        {
            get
            {
                if (PAR_Value >= 1)
                    return (int)(Height / PAR_Value);
                else
                    return Height;
            }
        }

        public string Extension
        {
            get
            {
                var Ext = File.Extension;
                string FName = "";
                if (Ext.Length > 0)
                {
                    FName = File.Name.Substring(0, File.Name.Length - File.Extension.Length);
                    Ext = Ext.Substring(2); // "FLV"
                }
                if (Ext == null)
                    Ext = "";
                return Ext;
            }
        }

        public bool IsImage
        {
            get
            {
                return FFMpegExtensions.ImageList.Contains(Extension.ToUpper());
            }
        }
        public bool IsVideo
        {
            get
            {
                return FFMpegExtensions.VideoList.Contains(Extension.ToUpper());
            }
        }
        public bool IsAudio
        {
            get
            {
                return FFMpegExtensions.AudioList.Contains(Extension.ToUpper());
            }
        }






        public VideoInfo()
        {

        }
        public VideoInfo(string fileName)
        {
            File = new FileInfo(fileName);
            this.Refresh();
            var a = 1;
        }
        public VideoInfo(FileInfo fileInfo)
        {
            File = fileInfo;
            this.Refresh();
        }




    }
}
