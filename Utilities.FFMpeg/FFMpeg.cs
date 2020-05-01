using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Utilities.EnumExtensions;
using Utilities.FileExtensions;

namespace Utilities.MediaConverter
{
    public class FFMpeg
    {
        public delegate void StateEventHandler(ProcessState PS);
        public delegate void StartConversionEventHandler(PassEnum TotalPasses, int CurrentPass);
        public delegate void EndConversionEventHandler();


        public event StateEventHandler State;


        public event StartConversionEventHandler StartConversion;


        public event EndConversionEventHandler EndConversion;


        public EncodingStateEnum EncodingState { get; set; } = EncodingStateEnum.Not_Encoding;

        public float MaxQualityFound = 0;

        private ShellCommand CMD = new ShellCommand();




        public FFMpeg()
        {
            CMD.DebugMessage += CMD_DebugMessage;
            CMD.ShellMessage += CMD_ShellMessage;

            Core.InitializationCheck();

        }
        private void CMD_DebugMessage(string Msg)
        {
            DebugMsg(Msg);
        }

        private void CMD_ShellMessage(string msg)
        {
            var Line = msg;
            if (string.IsNullOrEmpty(Line))
                return;
            Debug.WriteLine(Line);

            // Dim tstr() As String = Split(Line, "=")
            if (Line.Contains("time="))
            {
                EncodingState = EncodingStateEnum.Audio_Only;
                ProcessState PS = new ProcessState()
                {
                    Frame = Line.GetValueOnLine("frame").AsInt(),
                    FPS = Line.GetValueOnLine("fps").AsInt(),
                    Q = (float)Line.GetValueOnLine("Q").AsDouble(),
                    Size = Line.GetValueOnLine("size").AsInt(),
                    Time = (float)Line.GetValueOnLine("time").AsDouble(),
                    Bitrate = (float)Line.GetValueOnLine("bitrate").AsDouble()
                };
                if (PS.Frame > 0)
                    EncodingState = EncodingStateEnum.Video_and_Audio;
                if (PS.Q > 0)
                {
                    if (MaxQualityFound < PS.Q)
                        MaxQualityFound = PS.Q;
                }
                State?.Invoke(PS);
            }
        }


        public static void DebugMsg(string msg)
        {
            if (Core.DebugMessage != null)
            {
                Core.DebugMessage(msg);
            } else
            {
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " - " +  msg);
            }
        }


        public async Task RefreshAsync(VideoInfo video)
        {
            DebugMsg("RefreshAsync - " + Core.FFMpegLocation.FullName);
            var TempDirectory = new System.IO.DirectoryInfo(video.File.Directory.FullName + @"\.Temp");
            if (!TempDirectory.Exists)
                TempDirectory.Create();
            var FNWE = video.File.FileNameWithoutExtension();
            var TempVideoLogFile = new FileInfo(TempDirectory.FullName + @"\" + FNWE + "_Initial.log");

            if (TempVideoLogFile.Exists)
                TempVideoLogFile.Delete();
            DebugMsg("RefreshAsync - Getting Video Info");
            var task = RunFFMpegAsync("-i \"" + video.File.FullName + "\"");
            await Task.Delay(1000);
            DebugMsg("RefreshAsync - Awaiting Finishing of Refresh");
            var InfoData = await task;
            DebugMsg("RefreshAsync - Convert Finished");


            DebugMsg("RefreshAsync - Writing Info Data");
            var SW = new System.IO.StreamWriter(TempVideoLogFile.OpenWrite());
            SW.Write(InfoData);
            SW.Close();
            TempVideoLogFile.Refresh();
            DebugMsg("RefreshAsync - Processing Video Info");
            string[] Lines = InfoData.Split(System.Environment.NewLine.ToCharArray());

            foreach (var Line in Lines)
            {
                if (Line.Contains("Duration:"))
                {
                    var a = Line.IndexOf("Duration: ");
                    a = a + "Duration: ".Length;
                    var b = Line.IndexOf(", start:", a);
                    var Time = Line.Substring(a, b - a);
                    try
                    {
                        var TS = TimeSpan.Parse(Time);
                        video.Duration = TS.TotalMilliseconds / 1000;
                    }
                    catch (Exception ex)
                    {
                        video.Duration = 1000;
                    }
                }
                if (Line.Contains("Stream #"))
                {
                    if (Line.Contains("Video:"))
                    {
                        var a = Line.IndexOf("Video: ");
                        a = a + "Video: ".Length;
                        string[] Entries = Line.Substring(a).Split(',');
                        video.VideoEncoding = Entries.SafeGet(0).ToEnum<VideoEncodingEnum>();
                        video.VideoSubEncoding = Entries.SafeGet(1).ToEnum<VideoSubEncodingEnum>();
                        //Extensions.GetEnumData(typeof(VideoSubEncodingEnum), Extensions.GetPosition(Entries, 1));
                        var Extents = Entries.SafeGet(2);

                        string[] Tarr2 = Extents.Split('x');
                        if (Tarr2.Length >= 2)
                        {
                            video.Width = Tarr2[0].AsInt();
                            video.Height = Tarr2[1].AsInt();
                        }
                        if (Entries.SafeGet(3).Contains("kb/s"))
                            video.BitRate = Entries.SafeGet(3).AsInt();
                        if (Entries.SafeGet(3).Contains("tbr"))
                            video.FrameRate = Entries.SafeGet(3).AsDouble();
                        else if (Entries.SafeGet(4).Contains("tbr"))
                            video.FrameRate = Entries.SafeGet(4).AsDouble();
                        try
                        {
                            if (Extents.Contains("[") && Extents.Contains("]"))
                            {
                                var b = Extents.IndexOf("[");
                                var c = Extents.IndexOf("]");

                                string PARDAR = Extents.Substring(b + 1, (c - 1) - (b + 1) + 1);
                                string[] PD = PARDAR.Split(' ');
                                if (PD[0].ToUpper() == "PAR")
                                    video.PAR = PD[1];
                                else if (PD[2].ToUpper() == "PAR")
                                    video.PAR = PD[3];
                                if (PD[0].ToUpper() == "DAR")
                                    video.DAR = PD[1];
                                else if (PD[2].ToUpper() == "DAR")
                                    video.DAR = PD[3];
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        if (Entries.SafeGet(3).ToUpper().Contains("PAR"))
                        {
                            var Stuff = Entries.SafeGet(3).Trim();
                            string[] PD = Stuff.Split(' ');
                            if (PD[0].ToUpper() == "PAR")
                                video.PAR = PD[1];
                            else if (PD[2].ToUpper() == "PAR")
                                video.PAR = PD[3];
                            if (PD[0].ToUpper() == "DAR")
                                video.DAR = PD[1];
                            else if (PD[2].ToUpper() == "DAR")
                                video.DAR = PD[3];
                        }
                        else if (Entries.SafeGet(4).ToUpper().Contains("PAR"))
                        {
                            var Stuff = Entries.SafeGet(4).Trim();
                            string[] PD = Stuff.Split(' ');
                            if (PD[0].ToUpper() == "PAR")
                                video.PAR = PD[1];
                            else if (PD[2].ToUpper() == "PAR")
                                video.PAR = PD[3];
                            if (PD[0].ToUpper() == "DAR")
                                video.DAR = PD[1];
                            else if (PD[2].ToUpper() == "DAR")
                                video.DAR = PD[3];
                        }
                    }
                    else if (Line.Contains("Audio:"))
                    {
                        var a = Line.IndexOf("Audio: ");
                        a = a + "Audio: ".Length;
                        string[] Entries = Line.Substring(a).Split(',');
                        video.AudioEncoding = Entries.SafeGet(0).ToEnum<AudioEncodingEnum>();
                        video.AudioFrequency = Entries.SafeGet(1).AsInt();
                        video.Channels = Entries.SafeGet(2).ToEnum<SoundTypeEnum>();
                        if (video.Channels == SoundTypeEnum.None)
                        {
                            if (Entries.SafeGet(2).AsInt()>0)
                            {
                                video.Channels = SoundTypeEnum.Mono;
                            }
                        }
                        video.AudioBitRate = Entries.SafeGet(4).AsInt();
                    }
                }
            }

            DebugMsg("RefreshAsync - Video Info Loaded");
        }

        public async Task GenerateImageAsync(VideoInfo video, System.IO.FileInfo ImageFile, int Width, int Height)
        {
            if (ImageFile.Exists)
                ImageFile.Delete();

            var FNWE = video.File.FileNameWithoutExtension() + "_Image";
            var TempDirectory = new System.IO.DirectoryInfo(video.File.Directory.FullName + @"\.Temp\" + DateTime.Now.Year + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00"));
            if (!TempDirectory.Exists)
                TempDirectory.Create();
            System.IO.FileInfo TempLogFile = new System.IO.FileInfo(TempDirectory.FullName + @"\" + FNWE + ".log");
            if (TempLogFile.Exists)
                TempLogFile.Delete();

            DebugMsg("Grabbing Image");
            var LogData = await RunFFMpegAsync("-i \"" + video.File.FullName + "\" -an -ss 00:00:07 -an -s " + Width + "x" + Height + " -r 1 -vframes 1 -f image2 -y \"" + ImageFile.FullName + "\"");

            var SW = new System.IO.StreamWriter(TempLogFile.OpenWrite());
            SW.Write(LogData);
            SW.Close();

            DebugMsg("Image Grabbed");
            ImageFile.Refresh();
        }

        public async Task GenerateAudioAsync(VideoInfo video, System.IO.FileInfo Audiofile, AudioEncodingEnum AudioEncoding = AudioEncodingEnum.mp3, SoundTypeEnum Channels = SoundTypeEnum.Stereo, int AudioBitRate = 128, int AudioFrequency = 44100)
        {
            if (Audiofile.Exists)
                Audiofile.Delete();

            var FNWE = video.File.FileNameWithoutExtension() + "_Audio";
            var TempDirectory = new System.IO.DirectoryInfo(video.File.Directory.FullName + @"\.Temp\" + DateTime.Now.Year + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00"));
            if (!TempDirectory.Exists)
                TempDirectory.Create();
            System.IO.FileInfo TempLogFile = new System.IO.FileInfo(TempDirectory.FullName + @"\" + FNWE + ".log");
            if (TempLogFile.Exists)
                TempLogFile.Delete();

            DebugMsg("Grabbing Audio");
            var LogData = await RunFFMpegAsync("-i \"" + video.File.FullName + "\" -vn -acodec " + AudioEncoding.AudioFormat() + " -ab " + AudioBitRate + "k -ac " + System.Convert.ToInt32(Channels) + " -ar " + AudioFrequency + " -y \"" + Audiofile.FullName + "\"");
            var SW = new System.IO.StreamWriter(TempLogFile.OpenWrite());
            SW.Write(LogData);
            SW.Close();
            if (EncodingState == EncodingStateEnum.Not_Encoding)
                throw new Exception("No audio was encoded.\n\r" + LogData);
            DebugMsg("Audio Grabbed");
            Audiofile.Refresh();
        }
        public async Task<VideoInfo> ConvertToAsync(VideoInfo video, VideoConvertInfo newfile)
        {
            DebugMsg("Converting Video");
            DateTime STime = DateTime.Now;

            FileInfo FinalVideoFile = null;
            try
            {
                // Dim FInf = newfile.File
                //newfile.Duration = video.Duration;
                var FNWE = video.File.FileNameWithoutExtension();
                var FNWE2 = "";


                var TempDirectory = new System.IO.DirectoryInfo(video.File.Directory.FullName + @"\.Temp");
                if (!TempDirectory.Exists)
                    TempDirectory.Create();
                DebugMsg("Setting up file links");
                if (newfile.File == null)
                {
                    newfile.File = new System.IO.FileInfo(video.File.Directory.FullName + @"\" + FNWE + "." + newfile.VideoEncoding.ToString());
                    FNWE2 = FNWE;
                }
                else
                    FNWE2 = newfile.File.FileNameWithoutExtension();

                System.IO.FileInfo TempVideoFile = new System.IO.FileInfo(TempDirectory.FullName + @"\" + FNWE + newfile.File.Extension);
                // Dim TempImageFile As New System.IO.FileInfo(TempDirectory.FullName & "\" & FNWE & ".jpg")
                System.IO.FileInfo TempVideoLogFile = new System.IO.FileInfo(TempDirectory.FullName + @"\" + FNWE2 + "_" + newfile.VideoEncoding.ToString() + ".log");
                // Dim TempImageLogFile As New System.IO.FileInfo(TempDirectory.FullName & "\" & FNWE & "-Img.log")


                FinalVideoFile = newfile.File;
                // Dim FinalImageFile As New System.IO.FileInfo(newfile.File.Directory.FullName & "\" & FNWE2 & ".jpg")


                if (TempVideoFile.Exists)
                    TempVideoFile.Delete();
                // If TempImageFile.Exists Then
                // TempImageFile.Delete()
                // End If
                if (TempVideoLogFile.Exists)
                    TempVideoLogFile.Delete();
                // If TempImageLogFile.Exists Then
                // TempImageLogFile.Delete()
                // End If

                if (newfile.Width == 0 && newfile.Height == 0)
                {
                    newfile.Width = video.Width;
                    newfile.Height = video.Height;
                }

                string AdditionalString = "";

                DebugMsg("Currently    : " + video.Width + "x" + video.Height);
                DebugMsg("Currently Adj: " + video.WidthAdjusted + "x" + video.HeightAdjusted);
                DebugMsg("Going to    : " + newfile.Width + "x" + newfile.Height);
                DebugMsg("Going to Adj: " + newfile.WidthAdjusted + "x" + newfile.HeightAdjusted);
                var TopPad = 0;
                var BottomPad = 0;
                var LeftPad = 0;
                var RightPad = 0;
                if (!newfile.Stretch)
                {
                    if (video.WidthAdjusted > 0 && video.HeightAdjusted > 0)
                    {
                        if ((newfile.WidthAdjusted / (double)video.WidthAdjusted) > (newfile.HeightAdjusted / (double)video.HeightAdjusted))
                        {
                            // Y limited
                            TopPad = 0;
                            BottomPad = 0;
                            var AF = newfile.WidthAdjusted - video.WidthAdjusted * (newfile.HeightAdjusted / (double)video.HeightAdjusted);
                            var AF2 = System.Convert.ToInt32(AF / (double)2);
                            if (AF2 % 2 > 0)
                                AF2 += 1;
                            LeftPad = AF2;
                            RightPad = AF2;
                        }
                        else
                        {
                            // X Limited
                            LeftPad = 0;
                            RightPad = 0;
                            var AF = newfile.HeightAdjusted - video.HeightAdjusted * (newfile.WidthAdjusted / (double)video.WidthAdjusted);
                            var AF2 = System.Convert.ToInt32(AF / (double)2);
                            if (AF2 % 2 > 0)
                                AF2 += 1;
                            TopPad = AF2;
                            BottomPad = AF2;
                        }
                        if (TopPad > 0 || BottomPad > 0 || LeftPad > 0 || RightPad > 0)
                        {
                            if (newfile.VideoEncoding == VideoEncodingEnum.h264_iPod || newfile.VideoEncoding == VideoEncodingEnum.h264)
                                AdditionalString = " -padtop " + TopPad + " -padbottom " + BottomPad + " -padleft " + LeftPad + " -padright " + RightPad + " -padcolor 000000";
                            else
                                AdditionalString = " -vf pad=" + RightPad + ":" + BottomPad + ":" + LeftPad + ":" + TopPad + ":000000";
                        }
                    }
                }

                DebugMsg("Converting Video");
                string LogData = "";
                if (newfile.VideoEncoding == VideoEncodingEnum.flv)
                {
                    List<TestRun> TestList = new List<TestRun>();
                    TestList.Add(new TestRun() { Quality = (int)newfile.QMax, Passes = newfile.NumberPasses, BitRate = newfile.BitRate, MaxQuality = (int)newfile.QMax + 5 });
                    if (newfile.QMax > 0)
                    {
                        var QT = (int)newfile.QMax;
                        for (int a = 200; a <= 1000; a += 200)
                            TestList.Add(new TestRun() { Quality = QT, Passes = newfile.NumberPasses, BitRate = newfile.BitRate + a, MaxQuality = QT + 5 });
                        for (int a = 0; a <= 1000; a += 200)
                        {
                            TestList.Add(new TestRun() { Quality = QT + 5, Passes = newfile.NumberPasses, BitRate = newfile.BitRate + a, MaxQuality = QT + 8 });
                            TestList.Add(new TestRun() { Quality = QT + 10, Passes = newfile.NumberPasses, BitRate = newfile.BitRate + a, MaxQuality = QT + 11 });
                        }

                        TestList = (from TR in TestList
                                    orderby (Math.Pow(TR.MaxQuality, 2) + Math.Pow(TR.BitRate / 100.0, 2) * 0.65)
                                    select TR).ToList();

                        TestList.Add(new TestRun() { Quality = 0, Passes = PassEnum.Two, BitRate = newfile.BitRate + 1000, MaxQuality = 40 });
                        TestList.Add(new TestRun() { Quality = 0, Passes = PassEnum.One, BitRate = newfile.BitRate + 1000, MaxQuality = 40 });
                    }


                    TempVideoFile.Refresh();
                    int Cnt = 0;
                    DebugMsg("TestList.Count=" + TestList.Count);
                    while (((!TempVideoFile.Exists || TempVideoFile.Length < 1000) && (Cnt < TestList.Count)))
                    {
                        DebugMsg("Cnt=" + Cnt + " Q=" + TestList[Cnt].Quality + " Passes=" + TestList[Cnt].Passes.ToString());
                        newfile.QMax = TestList[Cnt].Quality;
                        newfile.NumberPasses = TestList[Cnt].Passes;
                        newfile.BitRate = TestList[Cnt].BitRate;

                        DebugMsg("MaxQuality looking for=" + TestList[Cnt].MaxQuality);
                        MaxQualityFound = 0;

                        string Arguments = "-i \"" + video.File.FullName + "\" -r " + newfile.FrameRate + " -f flv " + (newfile.Deinterlace ? "-deinterlace " : "") + "-ac " + System.Convert.ToInt32(newfile.Channels) + " -ar " + newfile.AudioFrequency + " -ab " + newfile.AudioBitRate + "k" + (newfile.AudioEncoding != AudioEncodingEnum.None ? " -acodec " + newfile.AudioEncoding.AudioFormat() : "") + " -b " + newfile.BitRate + "k -s " + (newfile.WidthAdjusted - LeftPad - RightPad) + "x" + (newfile.HeightAdjusted - TopPad - BottomPad) + " -aspect 16:9 ";
                        if (TestList[Cnt].Quality > 0)
                            Arguments = Arguments + "-qmin " + newfile.QMin + " -qmax " + newfile.QMax + " -qcomp 0.7 -g 299.7 -qdiff 4 ";
                        Arguments = Arguments + (AdditionalString + (video.IsAudio ? " -vn" : ""));
                        if (newfile.NumberPasses == PassEnum.Two)
                        {
                            MaxQualityFound = 0;
                            StartConversion?.Invoke(PassEnum.Two, 1);
                            LogData = await RunFFMpegAsync("-pass 1 " + Arguments + " -y \"" + TempVideoFile.FullName + "\"");

                            EndConversion?.Invoke();
                            MaxQualityFound = 0;
                            StartConversion?.Invoke(PassEnum.Two, 2);
                            LogData = await RunFFMpegAsync("-pass 2 " + Arguments + " -y \"" + TempVideoFile.FullName + "\"");

                            EndConversion?.Invoke();

                            if (EncodingState == EncodingStateEnum.Not_Encoding)
                            {
                            }
                        }
                        else
                        {
                            MaxQualityFound = 0;
                            StartConversion?.Invoke(PassEnum.One, 1);
                            LogData = await RunFFMpegAsync(Arguments + " -y \"" + TempVideoFile.FullName + "\"");
                            //newfile.EncodingState = video.EncodingState;
                            EndConversion?.Invoke();

                            if (EncodingState == EncodingStateEnum.Not_Encoding)
                                throw new Exception("Nothing was encoded.\n\r" + LogData);
                        }
                        Cnt += 1;
                        TempVideoFile.Refresh();
                        DebugMsg("MaxQualityFound=" + MaxQualityFound);
                        DebugMsg("TempVideoFile.Exists=" + TempVideoFile.Exists);
                        if (MaxQualityFound > (TestList[Cnt - 1].MaxQuality) && TempVideoFile.Exists && TempVideoFile.Length > 1000 && Cnt < TestList.Count)
                        {
                            DebugMsg("Video Created but not good enough quality");

                            TempVideoFile.Delete();
                            TempVideoFile.Refresh();
                        }


                        TempVideoLogFile.Refresh();
                        if (TempVideoLogFile.Exists)
                            TempVideoLogFile.Delete();
                        var SW2 = new System.IO.StreamWriter(TempVideoLogFile.OpenWrite());
                        SW2.Write(LogData);
                        SW2.Close();
                        TempVideoLogFile.Refresh();

                        DebugMsg("");
                    }
                    DebugMsg("TempVideoFile.Exists=" + TempVideoFile.Exists);
                }
                else if (newfile.VideoEncoding == VideoEncodingEnum.h264)
                {
                    MaxQualityFound = 0;
                    StartConversion?.Invoke(PassEnum.One, 1);
                    var task =  RunFFMpegOldAsync("-i \"" + video.File.FullName + "\" -r " + newfile.FrameRate + " -vcodec libx264 -threads 0 " + (newfile.Deinterlace ? "-deinterlace " : "") + "-ac " + System.Convert.ToInt32(newfile.Channels) + " -ar " + newfile.AudioFrequency + " -ab " + newfile.AudioBitRate + "k" + (newfile.AudioEncoding != AudioEncodingEnum.None ? " -acodec " + newfile.AudioEncoding.AudioFormat() : "") + " -s " + (newfile.WidthAdjusted - LeftPad - RightPad) + "x" + (newfile.HeightAdjusted - TopPad - BottomPad) + " -aspect 16:9 " + AdditionalString + (video.IsAudio ? " -vn" : "") + " -level 41 -crf 20 -bufsize 20000k -maxrate 25000k -g 250 -coder 1 -flags +loop -cmp +chroma -partitions +parti4x4+partp8x8+partb8x8 -flags2 +dct8x8+bpyramid -me_method umh -subq 7 -me_range 16 -keyint_min 25 -sc_threshold 40 -i_qfactor 0.71 -rc_eq 'blurCplx^(1-qComp)' -bf 16 -b_strategy 1 -bidir_refine 1 -refs 6 -deblockalpha 0 -deblockbeta 0 -y \"" + TempVideoFile.FullName + "\"");

                    DebugMsg("ConvertTo - Awaiting Finishing of Convert");
                    LogData = await task;
                    DebugMsg("ConvertTo - Convert Finished");
                    //newfile.EncodingState = video.EncodingState;
                    EndConversion?.Invoke();

                    if (EncodingState == EncodingStateEnum.Not_Encoding)
                        throw new Exception("Nothing was encoded.\n\r" + LogData);
                }
                else if (newfile.VideoEncoding == VideoEncodingEnum.h264_iPod)
                {
                    newfile.MaxQualityFound = 0;
                    StartConversion?.Invoke(PassEnum.One, 1);
                    LogData = await RunFFMpegOldAsync("-i \"" + video.File.FullName + "\" -r " + newfile.FrameRate + " -vcodec libx264 -threads 0 " + (newfile.Deinterlace ? "-deinterlace " : "") + "-ac " + System.Convert.ToInt32(newfile.Channels) + " -ar " + newfile.AudioFrequency + " -ab " + newfile.AudioBitRate + "k" + (newfile.AudioEncoding != AudioEncodingEnum.None ? " -acodec " + newfile.AudioEncoding.AudioFormat() : "") + " -s " + (newfile.WidthAdjusted - LeftPad - RightPad) + "x" + (newfile.HeightAdjusted - TopPad - BottomPad) + " -aspect " + (newfile.WidthAdjusted) + ":" + (newfile.HeightAdjusted) + " " + AdditionalString + (video.IsAudio ? " -vn" : "") + " -vpre \"" + Core.FFMpegLocation + @"ffpresets\libx264-ipod640.ffpreset"" -b " + newfile.BitRate + "k -bt " + newfile.BitRate + "k -f ipod -y \"" + TempVideoFile.FullName + "\"");

                    //newfile.EncodingState = video.EncodingState;
                    EndConversion?.Invoke();

                    if (EncodingState == EncodingStateEnum.Not_Encoding)
                        throw new Exception("Nothing was encoded.\n\r" + LogData);
                }
                else if (newfile.VideoEncoding == VideoEncodingEnum.OGG_Theora)
                {
                    // -f ogg -vcodec libtheora -b 800k -g 300 -acodec libvorbis -ab 128k

                    string Arguments = "-i \"" + video.File.FullName + "\" -threads 0 -g 300 " + "-qmin " + newfile.QMin + " -qmax " + newfile.QMax + (newfile.Deinterlace ? " -deinterlace " : "") + " -ac " + System.Convert.ToInt32(newfile.Channels) + " -vcodec libtheora -acodec libvorbis -ab 128k -b 800k -s " + (newfile.WidthAdjusted - LeftPad - RightPad) + "x" + (newfile.HeightAdjusted - TopPad - BottomPad) + " -aspect 16:9 ";

                    MaxQualityFound = 0;
                    StartConversion?.Invoke(PassEnum.Two, 1);
                    LogData = await RunFFMpegAsync("-pass 1 " + Arguments + " -y \"" + TempVideoFile.FullName + "\"");
                    //newfile.EncodingState = EncodingState;
                    EndConversion?.Invoke();
                    MaxQualityFound = 0;
                    StartConversion?.Invoke(PassEnum.Two, 2);
                    LogData = await RunFFMpegAsync("-pass 2 " + Arguments + " -y \"" + TempVideoFile.FullName + "\"");
                    //newfile.EncodingState = EncodingState;
                    EndConversion?.Invoke();
                    if (EncodingState == EncodingStateEnum.Not_Encoding)
                        throw new Exception("Nothing was encoded.\n\r" + LogData);
                }
                else if (newfile.VideoEncoding == VideoEncodingEnum.WebM)
                {
                    string Arguments = "-i \"" + video.File.FullName + "\" -threads 0 -keyint_min 0 -g 250 -skip_threshold 0 " + "-qmin " + newfile.QMin + " -qmax " + newfile.QMax + (newfile.Deinterlace ? " -deinterlace " : "") + " -ac " + System.Convert.ToInt32(newfile.Channels) + " -vcodec libvpx -acodec libvorbis -b 614400 -s " + (newfile.WidthAdjusted - LeftPad - RightPad) + "x" + (newfile.HeightAdjusted - TopPad - BottomPad) + " -aspect 16:9 ";

                    MaxQualityFound = 0;
                    StartConversion?.Invoke(PassEnum.Two, 1);
                    LogData = await RunFFMpegAsync("-pass 1 " + Arguments + " -y \"" + TempVideoFile.FullName + "\"");
                    //newfile.EncodingState = EncodingState;
                    EndConversion?.Invoke();
                    MaxQualityFound = 0;
                    StartConversion?.Invoke(PassEnum.Two, 2);
                    LogData = await RunFFMpegAsync("-pass 2 " + Arguments + " -y \"" + TempVideoFile.FullName + "\"");
                    //EncodingState = EncodingState;
                    EndConversion?.Invoke();

                    if (EncodingState == EncodingStateEnum.Not_Encoding)
                        throw new Exception("Nothing was encoded.\n\r" + LogData);
                }
                newfile.MaxQualityFound = MaxQualityFound;

                DebugMsg("Video Converted");
                TempVideoFile.Refresh();

                if (TempVideoLogFile.Exists)
                    TempVideoLogFile.Delete();
                var SW = new System.IO.StreamWriter(TempVideoLogFile.OpenWrite());
                SW.Write(LogData);
                SW.Close();
                TempVideoLogFile.Refresh();

                if (newfile.VideoEncoding == VideoEncodingEnum.flv)
                {
                    DebugMsg("Setting Meta Data");
                    LogData =await RunFLVToolAsync("-Uk \"" + TempVideoFile.FullName + "\"");
                }



                if (!FinalVideoFile.Directory.Exists)
                    FinalVideoFile.Directory.Create();



                DebugMsg("Handling Temp Video File");
                bool AllOK = true;
                if (TempVideoFile.Exists && TempVideoFile.Length > 1000)
                {
                    FinalVideoFile.Refresh();
                    if (FinalVideoFile.Exists)
                    {
                        FinalVideoFile.Delete();
                        FinalVideoFile.Refresh();
                    }
                    if (!FinalVideoFile.Exists)
                        TempVideoFile.MoveTo(FinalVideoFile.FullName);
                    else
                        DebugMsg("FINAL VIDEO FILE still exists");
                }
                else
                    AllOK = false;


                DebugMsg("Cleaning up directory");
                if (AllOK)
                {
                    var HisDir = new System.IO.DirectoryInfo(video.File.Directory.FullName + @"\.Temp\" + DateTime.Now.Year + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00"));
                    if (!HisDir.Exists)
                        HisDir.Create();
                    System.IO.FileInfo FF;
                    FF = new System.IO.FileInfo(HisDir.FullName + @"\" + TempVideoLogFile.Name);
                    if (FF.Exists)
                        FF.Delete();
                    TempVideoLogFile.MoveTo(FF.FullName);
                }
            }
            catch (Exception ex)
            {
                DebugMsg("Error: " + ex.ToString());
                throw;
            }

            DebugMsg("Time to convert:" + (DateTime.Now.Subtract(STime).TotalMilliseconds / 1000));
            Debug.WriteLine(DateTime.Now.Subtract(STime).TotalMilliseconds / 1000);


            return new VideoInfo(FinalVideoFile);
        }


        private  Task<string> RunFFMpegAsync(string Args)
        {
            EncodingState = EncodingStateEnum.Not_Encoding;
            return CMD.ShellAsync(Core.FFMpegExecutable.FullName, Args);
            //return CMD.Shell(Core.FFMpegExecutable.FullName, Args);
        }
        private Task<string> RunFFMpegOldAsync(string Args)
        {
            EncodingState = EncodingStateEnum.Not_Encoding;
            return CMD.ShellAsync(Core.FFMpegOldExecutable.FullName, Args);
            //return CMD.Shell(Core.FFMpegOldExecutable.FullName, Args);
        }
        private Task<string> RunFLVToolAsync(string Args)
        {
            return CMD.ShellAsync(Core.FLVToolExecutable.FullName, Args);
            //return CMD.Shell(Core.FLVToolExecutable.FullName, Args);
        }


    }
}
