using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Swagger.Abstract;
using Utilities.Swagger.Generators;
using Utilities.Swagger.Writers;

namespace Utilities.Swagger.Configs
{
    public class SwaggerSplitConfig : SwaggerGenProfile
    {

        public string ModulePaths = "/wwwroot/js/Api/Entities/";
        public string ModuleFilename = "{Name}.ts";
        public string EnumPaths = "/wwwroot/js/Api/Entities/";
        public string EnumFilename = "EnumDefinitions.ts";
        public string RepositoryPaths = "/wwwroot/js/Api/Repositories/";
        public string RepositoryFilename = "{Name}.repository.ts";


        public SwaggerSplitConfig() : base()
        {
            Name = "SplitApi";
        }

        internal override IFileGenerator GetFileGenerator(ILogger logger, ISwaggerGen swaggerFilterGen)
        {
            return new StandardTSGenerator(this, logger, swaggerFilterGen, new StandardMultiFileWriter(this, logger, swaggerFilterGen));
        }
    }
}
