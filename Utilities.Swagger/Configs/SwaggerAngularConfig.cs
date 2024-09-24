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
    public class SwaggerAngularConfig : SwaggerSplitConfig
    {

        public SwaggerAngularConfig(string appDirectory) : base()
        {
            Name = "AngularApi";

            ModulePaths = appDirectory + "/src/app/entities/";
            ModuleFilename = "{Name}.ts";
            EnumPaths = appDirectory + "/src/app/entities/";
            EnumFilename = "EnumDefinitions.ts";
            RepositoryPaths = appDirectory + "/src/app/repositories/";
            RepositoryFilename = "{Name}.repository.ts";
        }

        internal override IFileGenerator GetFileGenerator(ILogger logger, ISwaggerGen swaggerFilterGen)
        {
            return new StandardAngularGenerator(this, logger, swaggerFilterGen, new StandardMultiFileWriter(this, logger, swaggerFilterGen));
        }
    }
}
