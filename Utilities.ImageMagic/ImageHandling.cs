using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utilities.ImageMagic
{
    public class ImageHandling
    {

        public enum ImageType
        {
            None,
            JPG,
            PNG,
            BMP,
            GIF
        }
        public enum HAlignment_Enum
        {
            Center,
            Left,
            Right
        }
        public enum VAlignment_Enum
        {
            Center,
            Top,
            Bottom
        }

        private static string FileNameWithoutExtension(FileInfo File)
        {
            return File.Name.Substring(0, File.Name.Length - File.Extension.Length);
        }

        public static string GetResizedImage(string image, ImageSize imageSize)
        {
            return GetResizedImage(image, imageSize.Width, imageSize.Height, imageSize.Stretch, imageSize.ImageType, imageSize.FillArea, imageSize.HAlignment, imageSize.VAlignment, imageSize.JpegQuality);
        }
        public static string GetResizedImage(string image, int width, int height, bool stretch = false, ImageType imageType = ImageType.JPG, bool fillArea = false, HAlignment_Enum hAlignment = HAlignment_Enum.Center, VAlignment_Enum vAlignment = VAlignment_Enum.Center, int jpegQuality = 100)
        {
            var finImage = "";
            var cDir = Directory.GetCurrentDirectory();
            var fi = new FileInfo(Path.Combine(cDir, image));
            if (fi.Exists)
            {
                var fin = WebPathAndNameCached(fi, width, height, stretch, imageType, fillArea, hAlignment, vAlignment, jpegQuality);
                //Dim fi2 = GetFileInfo(fi, fi.Directory.FullName & "\TempFiles\" & fi.FileNameWithoutExtension() & "_thumbnail_" & width & "_" & height, "png")
                //If Not fi2.Directory.Exists Then
                //    fi2.Directory.Create()
                //End If
                //If Not fi2.Exists OrElse fi2.LastWriteTime.AddMinutes(15) < fi.LastWriteTime Then
                //    SaveResizedImage(fi, fi2, width, height, False, False, HAlignment_Enum.Center, VAlignment_Enum.Center)
                //End If

                var f = fi.FullName.Replace(image.Replace("//", "/").Replace("/", "\\"), "");
                var f2 = fin.FullName.Replace(f, "");


                finImage = f2.Replace("\\", "/");
            }


            return finImage;
        }


        public static FileInfo GetFileInfo(FileInfo origFile, string baseFileName, string extension)
        {

            if ((origFile.Extension.ToUpper() == ".SVG"))
            {
                return new FileInfo(baseFileName + ".svg");
            }
            if ((origFile.Extension.ToUpper() == ".GIF"))
            {
                return new FileInfo(baseFileName + ".gif");
            }
            return new FileInfo(baseFileName + "." + extension);
        }


        public static object WebPathAndNameCached(FileInfo BaseFile, ImageSize imageSize)
        {
            return WebPathAndNameCached(BaseFile, imageSize.Width, imageSize.Height, imageSize.Stretch, imageSize.ImageType, imageSize.FillArea, imageSize.HAlignment, imageSize.VAlignment);
        }

        public static System.IO.FileInfo WebPathAndNameCached(FileInfo BaseFile, int Width, int Height, bool Stretch, ImageType ImageType, bool FillArea, HAlignment_Enum HAlignment, VAlignment_Enum VAlignment, int jpegQuality = 100)
        {
            System.IO.FileInfo FI2 = default(System.IO.FileInfo);
            var FI = BaseFile;

            var Fname = FileNameWithoutExtension(FI);

            var Extension = FI.Extension;

            if (ImageType != ImageType.None)
            {
                Extension = ImageType.ToString();
            }
            else
            {
                try
                {
                    ImageType =(ImageType) Enum.Parse(typeof(ImageType), Extension.Replace(".", "").ToUpper());

                }
                catch (Exception exp)
                {
                }
            }

            if (ImageType == ImageType.None)
            {
                ImageType = ImageType.JPG;
            }

            if (Width > 0)
            {
                //If Width > 1024 Then Width = 1024
                Fname = Fname + "_" + Width;
            }
            else if (Height > 0)
            {
                //If Height > 1024 Then Height = 1024
                Fname = Fname + "_0_" + Height;
            }

            if (Height > 0 && Width > 0)
            {
                //If Height > 1024 Then Height = 1024
                Fname = Fname + "_" + Height;
            }
            //If Not Stretch.HasValue Then
            //    Stretch = False
            //End If
            if (Stretch)
            {
                Fname = Fname + "_Stretch";
            }
            if (FillArea)
            {
                Fname = Fname + "_FillArea";
            }
            if (HAlignment != HAlignment_Enum.Center)
            {
                Fname = Fname + "_H_" + HAlignment.ToString();
            }
            if (VAlignment != VAlignment_Enum.Center)
            {
                Fname = Fname + "_V_" + VAlignment.ToString();
            }

            if (ImageType != ImageType.None)
            {
                Extension = "." + ImageType.ToString();
            }

            FI2 = new System.IO.FileInfo(FI.DirectoryName + "/Temp/" + Fname + Extension);
            if (!FI2.Directory.Exists)
            {
                FI2.Directory.Create();
            }
            if (!(FI2.Exists && (FI.LastWriteTime <= FI2.LastWriteTime || FI.FullName.ToUpper() == FI2.FullName.ToUpper())))
            {
                SaveResizedImage(FI, FI2, (Width > 0 ? (int?)Width : null), (Height > 0 ? (int?)Height : null), Stretch, FillArea, HAlignment, VAlignment, jpegQuality);
                FI2.Refresh();
            }

            return FI2;
        }

        public static void SaveResizedImage(System.IO.FileInfo FromFile, System.IO.FileInfo ToFile, ImageSize imageSize)
        {
            SaveResizedImage(FromFile, ToFile, imageSize.Width, imageSize.Height, imageSize.Stretch, imageSize.FillArea, imageSize.HAlignment, imageSize.VAlignment, imageSize.JpegQuality);
        }
        public static void SaveResizedImage(System.IO.FileInfo FromFile, System.IO.FileInfo ToFile, Nullable<int> Width, Nullable<int> Height, bool Stretch, bool FillArea, HAlignment_Enum HAlignment, VAlignment_Enum VAlignment, int jpegQuality = 100)
        {

            try
            {
                if (ToFile.Exists && ToFile.FullName.ToUpper() != FromFile.FullName.ToUpper())
                    ToFile.Delete();
                var Extension = ToFile.Extension.Replace(".", "");
                bool UseTransparent = false;
                if (Extension.ToUpper() == "SVG")
                {
                    FromFile.CopyTo(ToFile.FullName);
                    return;
                }
                if (Extension.ToUpper() == "PNG")
                {
                    UseTransparent = true;
                }
                if ((Extension.ToUpper() == "GIF"))
                {
                    UseTransparent = true;
                    var img = Image.Load(FromFile.FullName);
                    var frames = img.Frames.Count;
                    if ((frames > 1))
                    {
                        FromFile.CopyTo(ToFile.FullName);
                        return;
                    }
                }

                if (!Width.HasValue && !Height.HasValue)
                {
                    FromFile.CopyTo(ToFile.FullName);
                    return;
                }

                using (var Ti = Image.Load(FromFile.FullName)) {
                    bool BothSet = true;
                    if (Width.HasValue & !Height.HasValue)
                    {
                        Height = Ti.Height / Ti.Width * Width;
                        BothSet = false;
                    }
                    if (Height.HasValue & !Width.HasValue)
                    {
                        Width = Ti.Width / Ti.Height * Height;
                        BothSet = false;
                    }

                    using (var FT = new Image<Rgba32>(Width ?? 0, Height ?? 0))
                    {
                        if (UseTransparent)
                        {
                            FT.Mutate(ctx => ctx.Fill(Rgba32.Transparent));
                        }
                        else
                        {
                            FT.Mutate(ctx => ctx.Fill(Rgba32.WhiteSmoke));
                        }

                        if (BothSet)
                        {
                            var NWidth = Width;
                            var NHeight = Height;


                            var dx = Width / Ti.Width;
                            var dy = Height / Ti.Height;
                            bool Comp = (dx < dy);
                            var Ratio = Ti.Width / Ti.Height;
                            //If Ratio > 1 Then Ratio = 1 / Ratio





                            if (!Stretch)
                            {
                                if (FillArea)
                                    Comp = !Comp;
                                if (Comp)
                                {
                                    var y = (int)(dx * Ti.Height);
                                    var T = (int)((Height - y) / 2);
                                    if (VAlignment == VAlignment_Enum.Top)
                                    {
                                        T = 0;
                                    }
                                    if (VAlignment == VAlignment_Enum.Bottom)
                                    {
                                        T = (Height ?? 0) - y;
                                    }
                                    FT.Mutate(ctx => ctx.DrawImage(Ti,1,new Size(Width ?? 0, y), new Point(0, T)));

                                    //Gr = System.Drawing.Graphics.FromImage(FT);
                                    //Gr.DrawImage(Ti, new Drawing.Rectangle(0, T, Width, y));
                                    //Gr.Dispose();
                                    //Ti = FT;
                                }
                                else
                                {
                                    var x = (int)(dy * Ti.Width);
                                    var L = (int)((Width - x) / 2);
                                    if (HAlignment == HAlignment_Enum.Left)
                                    {
                                        L = 0;
                                    }
                                    if (HAlignment == HAlignment_Enum.Right)
                                    {
                                        L = (Width ?? 0) - x;
                                    }
                                    FT.Mutate(ctx => ctx.DrawImage(Ti, 1, new Size(x, Height ?? 0), new Point(L,  0)));
                                    //Gr = System.Drawing.Graphics.FromImage(FT);
                                    //Gr.DrawImage(Ti, new Drawing.Rectangle(L, 0, x, Height));
                                    ////Gr.DrawImageUnscaled(Ti, New Drawing.Point(0, 0))
                                    //Gr.Dispose();
                                    //Ti = FT;
                                }
                            }
                            else
                            {
                                FT.Mutate(ctx => ctx.DrawImage(Ti, 1, new Size(Width ?? 0, Height ?? 0), new Point(0, 0)));
                                //Gr = System.Drawing.Graphics.FromImage(FT);
                                //Gr.DrawImage(Ti, new Drawing.Rectangle(0, 0, Width, Height));
                                //Gr.Dispose();
                                //Ti = FT;
                            }
                        }
                        else
                        {
                            FT.Mutate(ctx => ctx.DrawImage(Ti, 1, new Size(Width ?? 0, Height ?? 0), new Point(0, 0)));
                            //Gr = System.Drawing.Graphics.FromImage(FT);
                            //Gr.DrawImage(Ti, new Drawing.Rectangle(0, 0, Width, Height));
                            //Gr.Dispose();
                            //Ti = FT;
                        }

                        if (ToFile.Exists && ToFile.FullName.ToUpper() == FromFile.FullName.ToUpper())
                            ToFile.Delete();


                        FT.Save(ToFile.FullName);
                        //if (Extension.ToUpper() == "JPG" | Extension.ToUpper() == "JPEG")
                        //{
                        //    // Encoder parameter for image quality
                        //    EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, jpegQuality);
                        //    // Jpeg image codec 
                        //    ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                        //    EncoderParameters encoderParams = new EncoderParameters(1);
                        //    encoderParams.Param(0) = qualityParam;
                        //    FT.Save(ToFile.FullName, jpegCodec, encoderParams);
                        //    //Ti.Save(ToFile.FullName, Drawing.Imaging.ImageFormat.Jpeg)
                        //}
                        //if (Extension.ToUpper() == "PNG")
                        //{
                        //    FT.Save(ToFile.FullName, Drawing.Imaging.ImageFormat.Png);
                        //}
                        //if (Extension.ToUpper() == "BMP")
                        //{
                        //    FT.Save(ToFile.FullName, Drawing.Imaging.ImageFormat.Bmp);
                        //}
                        //if (Extension.ToUpper() == "GIF")
                        //{
                        //    FT.Save(ToFile.FullName, Drawing.Imaging.ImageFormat.Gif);
                        //}
                        //if (Extension.ToUpper() == "TGA")
                        //{
                        //    FT.Save(ToFile.FullName, Drawing.Imaging.ImageFormat.Jpeg);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                int i = 1;
            }

        }

        //// Returns the image codec with the given mime type 
        //private static ImageCodecInfo GetEncoderInfo(string mimeType)
        //{
        //    // Get image codecs for all image formats 
        //    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        //    // Find the correct image codec 
        //    for (int i = 0; i <= codecs.Length - 1; i++)
        //    {
        //        if ((codecs(i).MimeType == mimeType))
        //        {
        //            return codecs(i);
        //        }
        //    }

        //    return null;
        //}
    }
}
