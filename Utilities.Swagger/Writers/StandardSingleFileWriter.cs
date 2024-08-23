using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Swagger.Abstract;
using Utilities.Swagger.Configs;

namespace Utilities.Swagger.Writers
{
    public class StandardSingleFileWriter : StandardWriter
    {
        private readonly SwaggerStandardConfig _config;
        private readonly ISwaggerGen _swaggerFilterGen;

        public StandardSingleFileWriter(SwaggerStandardConfig config, ILogger logger, ISwaggerGen swaggerFilterGen) :base(logger) 
        {
            _config = config;
            _swaggerFilterGen  = swaggerFilterGen;
        }

        private FileEntry GetIndexFile()
        {
            if (!(_config.Path.EndsWith("/") || _config.Path.EndsWith("\\")))
            {
                _config.Path = _config.Path + "/";
            }
            var fi = new FileInfo(_swaggerFilterGen.MapPath(_config.Path + "index.ts"));

            try
            {
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                if (fi.Exists && DateTime.Now.Subtract(fi.LastWriteTime).TotalMinutes > 5)
                {

                    fi.Delete();
                }
            } catch
            {

            }

            return new FileEntry()
            {
                File = fi,
                ConfigDirectory = _config.Path
            };
        }
        private FileEntry GetFile()
        {
            if (!(_config.Path.EndsWith("/") || _config.Path.EndsWith("\\")))
            {
                _config.Path = _config.Path + "/";
            }
            var fi = new FileInfo(_swaggerFilterGen.MapPath(_config.Path + _config.Filename));

            try
            {
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                if (fi.Exists && DateTime.Now.Subtract(fi.LastWriteTime).TotalMinutes > 5)
                {
                    fi.Delete();
                }
            }
            catch
            {

            }

            return new FileEntry()
            {
                File = fi,
                ConfigDirectory = _config.Path
            };
        }

        public override bool NamespacesInOneFile => true;

        public override FileEntry GetModuleIndex()
        {
            return GetIndexFile();
        }

        public override FileEntry GetNamespaceIndex()
        {
            return GetIndexFile();
        }

        public override FileEntry GetEnumIndex()
        {
            return GetIndexFile();
        }
        public override FileEntry GetEnumFile(string name)
        {
            return GetFile();
        }

        public override string GetEnumToModuleImportLocation(string name)
        {
            return "";
        }

        public override string GetEnumToRepoImportLocation(string name)
        {
            return "";
        }

        public override FileEntry GetModuleFile(string name)
        {
            return GetFile();
        }

        public override string GetModuleToModuleImportLocation(string name)
        {
            return "";
        }

        public override string GetModuleToRepoImportLocation(string name)
        {
            return "";
        }

        public override FileEntry GetNamespaceFile(string namespaceName)
        {
            return GetFile();
        }

    }
}
