using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.EnumExtensions;
using Utilities.FileExtensions;

namespace Utilities.MediaConverter
{
    public static class Core
    {

        public static Action<string> DebugMessage { get; set; }

        public static void DebugMsg(string msg)
        {
            if (DebugMessage != null)
            {
                DebugMessage(msg);
            }
            else
            {
                Debug.WriteLine(DateTime.Now.ToShortTimeString() + " - " + msg);
            }
        }

        public static string FFMpegBaseDirectory { get; set; }

        public static DirectoryInfo FFMpegLocation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FFMpegBaseDirectory))
                {
                    var ass = Assembly.GetExecutingAssembly();
                    var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    FFMpegBaseDirectory = fi.Directory.FullName + @"\ffmpeg\";
                }
                return new DirectoryInfo(FFMpegBaseDirectory);
            }
        }

        public static FileInfo FFMpegExecutable => new FileInfo(FFMpegLocation.FullName + @"\ffmpeg.exe");
        public static FileInfo FFMpegOldExecutable => new FileInfo(FFMpegLocation.FullName + @"\ffmpeg_old.exe");
        public static FileInfo FLVToolExecutable => new FileInfo(FLVToolLocation.FullName + @"\FLVTool2.exe");


        public static DirectoryInfo FLVToolLocation
        {
            get
            {
                return FFMpegLocation;
            }
        }


        public static async Task GenerateImageAsync(this VideoInfo video, System.IO.FileInfo imageFile, int width, int height)
        {

            var ffmpeg = new FFMpeg();
            await ffmpeg.GenerateImageAsync(video, imageFile, width, height);
        }

        public static async Task GenerateAudioAsync(this VideoInfo video, System.IO.FileInfo audiofile, AudioEncodingEnum audioEncoding = AudioEncodingEnum.mp3, SoundTypeEnum channels = SoundTypeEnum.Stereo, int audioBitRate = 128, int audioFrequency = 44100)
        {

            var ffmpeg = new FFMpeg();
            await ffmpeg.GenerateAudioAsync(video, audiofile, audioEncoding, channels, audioBitRate, audioFrequency);
        }



        public static async Task<VideoInfo> ConvertToAsync(this VideoInfo video, VideoConvertInfo newfile)
        {

            var ffmpeg = new FFMpeg();
            return await ffmpeg.ConvertToAsync(video, newfile);
        }


        internal static void InitializationCheck()
        {

            //if (ffMpegPath.IsNullOrWhiteSpace())
            //{
            //    ffMpegPath = DefaultFFmpegFilePath;
            //}

            //this.FFmpegFilePath = ffMpegPath;
            var s = FFMpegLocation;


            EnsureDirectoryExists();
            EnsureFFmpegFileExists();
            //EnsureFFmpegIsNotUsed();
        }

        private static void EnsureDirectoryExists()
        {
            //string directory = FFMpegLocation?.FullName ?? Directory.GetCurrentDirectory(); ;

            if (!FFMpegLocation.Exists)
            {
                FFMpegLocation.Create();
            }
            FFMpegLocation.Refresh();
        }

        private static void EnsureFFmpegFileExists()
        {
            if (!FFMpegExecutable.Exists || !FFMpegOldExecutable.Exists || !FLVToolExecutable.Exists)
            {
                FFMpegLocation.Delete(true);
                FFMpegLocation.Create();




                UnpackFFmpegExecutable(FFMpegLocation);
            }
            FFMpegExecutable.Refresh();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Unpack ffmpeg executable. </summary>
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        private static void UnpackFFmpegExecutable(DirectoryInfo path)
        {
            Stream compressedFFmpegStream = Assembly.GetExecutingAssembly()
                                                    .GetManifestResourceStream("Utilities.MediaConverter.Resources.ffmpeg.zip");

            if (compressedFFmpegStream == null)
            {
                throw new Exception("ffmpeg not found");
            }

            using (ZipArchive archive = new ZipArchive(compressedFFmpegStream))
            {
                archive.ExtractToDirectory(path.FullName);
            }

        }





    }
}
