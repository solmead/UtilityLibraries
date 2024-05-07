using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.PdfHandling.Models
{
    //public enum PageOrientation
    //{
    //    Portrait,
    //    Landscape
    //}
    internal class PdfCreateRequest
    {
        public string Url { get; set; }
        public PageOrientation Orientation { get; set; } = PageOrientation.Portrait;
    }
}
