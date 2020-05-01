using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.MediaConverter
{
    public enum EncodingStateEnum
    {
        Not_Encoding,
        Audio_Only,
        Video_Only,
        Video_and_Audio
    }

    public enum SoundTypeEnum
    {
        None = 0,
        Mono = 1,
        Stereo = 2
    }

    public enum VideoSubEncodingEnum
    {
        None,
        yuv420p
    }

    public enum VideoEncodingEnum
    {
        None,
        mpeg2video,
        mpeg4,
        flv,
        h264,
        h264_iPod,
        OGG_Theora,
        WebM
    }

    public enum AudioEncodingEnum
    {
        None,
        mp3,
        ac3,
        mp2,
        aac
    }

    public enum PassEnum
    {
        One = 1,
        Two = 2
    }

}
