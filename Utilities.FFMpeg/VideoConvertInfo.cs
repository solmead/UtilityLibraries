using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.MediaConverter
{
    public class VideoConvertInfo //: VideoInfo
    {
        public VideoConvertInfo()
        {

        }
        public VideoConvertInfo(VideoInfo originalVideo)
        {
            Width = 0;
            Height = 0;
            PAR = originalVideo.PAR;
            File = null;

        }

        public bool Stretch { get; set; } = false;



        public float MaxQualityFound = 0;

        public float QMax = 5;
        public float QMin = 2;
        public PassEnum NumberPasses = PassEnum.Two;
        public bool Deinterlace = false;

        public VideoEncodingEnum VideoEncoding { get; set; } = VideoEncodingEnum.h264;
        public int BitRate { get; set; } = 15000;
        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public double FrameRate { get; set; } = 24; // 29.97
        public SoundTypeEnum Channels { get; set; } = SoundTypeEnum.Stereo;

        public int AudioBitRate { get; set; } = 128;
        public AudioEncodingEnum AudioEncoding { get; set; } = AudioEncodingEnum.aac;
        public int AudioFrequency { get; set; } = 44100;



        public string PAR = "1:1";
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
                    if (v2 == 0)
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

        public System.IO.FileInfo File { get; set; }

    }
}
