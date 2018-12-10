using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.ImageMagic
{
    public class ImageSize
    {


        public int Width { get; set; }
        public int Height { get; set; }
        public bool Stretch { get; set; }
        public ImageHandling.ImageType ImageType { get; set; }
        public bool FillArea { get; set; }
        public ImageHandling.HAlignment_Enum HAlignment { get; set; }
        public ImageHandling.VAlignment_Enum VAlignment { get; set; }

        public int JpegQuality { get; set; }


    }
}
