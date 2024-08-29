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
    public class StandardMultiFileWriter : StandardWriter
    {
        private readonly SwaggerSplitConfig _config;

        public StandardMultiFileWriter(SwaggerSplitConfig config, ILogger logger, ISwaggerGen swaggerFilterGen) : base(logger, swaggerFilterGen)
        {
            _config = config;
        }

        private FileEntry GetFile(string path, string filename, string name)
        {
            if (!(path.EndsWith("/") || path.EndsWith("\\")))
            {
                path = path + "/";
            }
            path = path.Replace("\\", "/");
            //path = path.Replace('/', Path.PathSeparator);

            var fname = filename.Replace("{Name}", name);


            var fi = new FileInfo(_swaggerFilterGen.MapPath(path + fname));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            if (fi.Exists && DateTime.Now.Subtract(fi.LastWriteTime).TotalMinutes > 5)
            {

                fi.Delete();
            }

            return new FileEntry()
            {
                File = fi,
                ConfigDirectory = path
            };
        }
        private FileEntry GetIndexFile(string path)
        {
            if (!(path.EndsWith("/") || path.EndsWith("\\")))
            {
                path = path + "/";
            }
            path = path.Replace("\\", "/");
            //path = path.Replace('/', Path.PathSeparator);

            var fi = new FileInfo(_swaggerFilterGen.MapPath(path + "index.ts"));
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            if (fi.Exists && DateTime.Now.Subtract(fi.LastWriteTime).TotalMinutes > 5)
            {

                fi.Delete();
            }

            return new FileEntry()
            {
                File = fi,
                ConfigDirectory = path
            };
        }

        public override FileEntry GetModuleIndex()
        {
            return GetIndexFile(_config.ModulePaths);
        }

        public override FileEntry GetNamespaceIndex()
        {
            return GetIndexFile(_config.RepositoryPaths);
        }

        public override FileEntry GetEnumIndex()
        {
            return GetIndexFile(_config.EnumPaths);
        }



        public override bool NamespacesInOneFile => false;

        public override FileEntry GetEnumFile(string name)
        {
            return GetFile(_config.EnumPaths, _config.EnumFilename, name);
        }
        public override FileEntry GetModuleFile(string name)
        {
            return GetFile(_config.ModulePaths, _config.ModuleFilename, name);
        }
        public override FileEntry GetNamespaceFile(string namespaceName)
        {
            return GetFile(_config.RepositoryPaths, _config.RepositoryFilename, namespaceName);
        }


        public override string GetEnumToModuleImportLocation(string name)
        {
            if (_config.EnumPaths == _config.ModulePaths)
            {
                return "./" + _config.EnumFilename.Replace("{Name}", name).Replace(".ts", "") + "";
            } else
            {
                return FindRelativePath(_config.ModulePaths, _config.EnumPaths) + "" + _config.EnumFilename.Replace("{Name}", name).Replace(".ts", "") + "";
            }
        }

        public override string GetEnumToRepoImportLocation(string name)
        {
            if (_config.EnumPaths == _config.RepositoryPaths)
            {
                return "./" + _config.EnumFilename.Replace("{Name}", name).Replace(".ts", "") + "";
            }
            else
            {
                return FindRelativePath(_config.RepositoryPaths, _config.EnumPaths) + "" + _config.EnumFilename.Replace("{Name}", name).Replace(".ts", "") + "";
            }
        }


        public override string GetModuleToModuleImportLocation(string name)
        {
            return "./" + _config.ModuleFilename.Replace("{Name}", name).Replace(".ts", "") + "";
        }

        public override string GetModuleToRepoImportLocation(string name)
        {
            if (_config.ModulePaths == _config.RepositoryPaths)
            {
                return "./" + _config.ModuleFilename.Replace("{Name}", name).Replace(".ts", "") + "";
            }
            else
            {
                return FindRelativePath(_config.RepositoryPaths, _config.ModulePaths) + "" + _config.ModuleFilename.Replace("{Name}", name).Replace(".ts", "") + "";
            }
        }

    }
}
