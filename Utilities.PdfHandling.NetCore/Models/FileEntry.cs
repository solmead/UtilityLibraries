using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Utilities.PdfHandling.Models
{
    public class FileEntry
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("data")]
        public byte[] Data { get; set; }
    }
}
