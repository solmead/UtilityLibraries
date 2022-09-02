using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileExtensions.AspNetCore.Configuration
{
    public class FileServerProviderOptions
    {
        public List<DirectorySetting> NetworkDirectories { get; set; } = new List<DirectorySetting>(); 
    }

    public class DirectorySetting
    {
        public string WebPath { get; set; }
        public string NetworkPath { get; set; }

        public DirectorySetting()
        {

        }

        public DirectorySetting(string webPath, string networkPath)
        {
            WebPath = webPath;
            NetworkPath = networkPath;
        }   
    }
}
