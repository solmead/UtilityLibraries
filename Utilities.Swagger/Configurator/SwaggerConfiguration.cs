using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using Utilities.FileExtensions;
using Utilities.Swagger;
using Utilities.Swagger.Configs;

namespace Utilities.Swagger.Configurator
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                Enum.GetNames(context.Type)
                    .ToList()
                    .ForEach(name => model.Enum.Add(new OpenApiString($"{name} = {Convert.ToInt64(Enum.Parse(context.Type, name))}")));
            }
        }
    }
    public static class SwaggerConfiguration
    {

        internal static Dictionary<string, SwaggerGenProfile> profileServices = new Dictionary<string, SwaggerGenProfile>();

        public static SwaggerGenProfile getProfile(string name)
        {
            name = name.ToUpper();
            if (profileServices.ContainsKey(name))
            {
                return profileServices[name];
            }
            if (profileServices.Count > 0)
            {
                return profileServices[profileServices.Keys.First()];
            }
            return null;
        }
        public static void addProfile(string name, SwaggerGenProfile profile)
        {
            name = name.ToUpper();
            if (profileServices.ContainsKey(name))
            {
                profileServices[name] = profile;
            }
            else
            {
                profileServices.Add(name, profile);
            }
        }





        public static IServiceCollection AddSwaggerApiVersion(this IServiceCollection services, 
            Action<ApiVersioningOptions>? setupVersioningAction = null,
            Action<ApiExplorerOptions>? setupExplorerAction = null)
        {
            var defaultVersion = new ApiVersion(1, 0);

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = defaultVersion;

                if (setupVersioningAction!=null)
                {
                    setupVersioningAction(o);
                }
            })
            .AddMvc()
            .AddApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;

                    options.FormatGroupName = (group, version) => $"{version}";


                    if (setupExplorerAction != null)
                    {
                        setupExplorerAction(options);
                    }
                });

            return services;
        }


        public static IApplicationBuilder UseStandardSwagger(this IApplicationBuilder app,
            ILogger logger,
            IApiVersionDescriptionProvider provider,
            string siteDirectory,
            string siteName,
            Action<SwaggerUIOptions>? setupAction = null)
        {

            if (!string.IsNullOrWhiteSpace(siteDirectory))
            {
                siteDirectory = "/" + siteDirectory;

            }
            siteDirectory.Replace("\\", "/").Replace("//", "/");

            ConfigureSwaggerOptions.ProjectName = siteName;

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.

            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var a = $"{description.GroupName}/swagger.json";
                        logger.LogInformation("UseStandardSwagger swagger endpoint = [" + a + "] groupName = [" + description.GroupName.ToUpperInvariant() + "]");
                        //options.SwaggerEndpoint($"/{settings.SiteDirectory}/Documents/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        options.SwaggerEndpoint(a, description.GroupName.ToUpperInvariant());
                    }
                    options.DisplayOperationId();
                    options.DisplayRequestDuration();
                    options.RoutePrefix = "swagger"; // {siteDirectory} "swagger";

                    if (setupAction!= null)
                    {
                        setupAction(options);
                    }

                });

            return app;
        }

        public static IServiceCollection RegisterSwagger(this IServiceCollection services, IHostEnvironment env, IConfiguration configuration, ILogger logger, LocalFileHandler localFileHandler, string tempDirectory)
        {
            var lst = new List<SwaggerGenProfile>();
            var ass = Assembly.GetCallingAssembly();
            var xmlFilename = $"{ass.GetName().Name}.xml";

            return services.AddSwaggerGenCustom(env, configuration, logger, localFileHandler, tempDirectory, xmlFilename, lst);
        }
        public static IServiceCollection RegisterSwagger(this IServiceCollection services, IHostEnvironment env, IConfiguration configuration, ILogger logger, LocalFileHandler localFileHandler, string tempDirectory,
            SwaggerGenProfile? options = null)
        {
            var lst = new List<SwaggerGenProfile>();
            if (options!=null)
            {
                lst.Add(options);
            }

            var ass = Assembly.GetCallingAssembly();
            var xmlFilename = $"{ass.GetName().Name}.xml";

            return services.AddSwaggerGenCustom(env, configuration, logger, localFileHandler, tempDirectory, xmlFilename, lst);
        }


        public static IServiceCollection RegisterSwagger(this IServiceCollection services, IHostEnvironment env, IConfiguration configuration, ILogger logger, LocalFileHandler localFileHandler, string tempDirectory,
            List<SwaggerGenProfile>? options = null)
        {
            options = options ?? new List<SwaggerGenProfile>();

            if (!options.Any())
            {
                options.Add(new SwaggerStandardConfig());
            }

            var ass = Assembly.GetCallingAssembly();
            var xmlFilename = $"{ass.GetName().Name}.xml";

            services.AddSwaggerGenCustom(env, configuration, logger, localFileHandler, tempDirectory, xmlFilename, options);


            return services;
        }


        /// <summary>
        /// Custom extension for configuring Swagger.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static IServiceCollection AddSwaggerGenCustom(this IServiceCollection services, 
            IHostEnvironment env, 
            IConfiguration configuration, 
            ILogger _logger, 
            LocalFileHandler localFileHandler, 
            string tempDirectory, 
            string xmlFilename,
            List<SwaggerGenProfile>? options = null)
        {
            options = options ?? new List<SwaggerGenProfile>();

            if (!options.Any())
            {
                options.Add(new SwaggerStandardConfig());
            }


            //services.AddMvcCore()
            //    .AddJsonFormatters()
            //    .AddVersionedApiExplorer(
            //  options =>
            //  {
            //      //The format of the version added to the route URL  
            //      options.GroupNameFormat = "'v'VVV";
            //      //Tells swagger to replace the version in the controller route  
            //      options.SubstituteApiVersionInUrl = true;
            //  }); ;

            //services.AddApiVersioning(options => options.ReportApiVersions = true);

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();


            // Register the Swagger generator, defining 1 or more Swagger documents
            // Call c.SwaggerDoc to add more versions if nessesary.
            // Version declarations must be defined as v{versionNumber} to work properly with the AspNetCore.Mvc.Versioning library.
            services.AddSwaggerGen(c =>
            {
                _logger.LogInformation("AddSwaggerGen");
                // add a custom operation filter which sets default values
                c.OperationFilter<SwaggerDefaultValues>();

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());


                // Resolve the temprary IApiVersionDescriptionProvider service  
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                c.CustomSchemaIds(x => x.Name);

                c.SchemaFilter<EnumSchemaFilter>();

                tempDirectory = tempDirectory ?? AppContext.BaseDirectory;
               // var xmlCommentsFile = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                var xmlCommentsFile = Path.Combine(tempDirectory, xmlFilename);
                _logger.LogInformation("xmlCommentsFile = [" + xmlCommentsFile + "]");
                var f = localFileHandler.GetFileInfo(tempDirectory, xmlFilename);
                _logger.LogInformation("xmlCommentsFile = [" + f.FullName + "]");
                xmlCommentsFile = f.FullName;


                if (!string.IsNullOrWhiteSpace(xmlCommentsFile))
                {

                    c.IncludeXmlComments(xmlCommentsFile);
                }


                _logger.LogInformation("DocInclusionPredicate");
                // Ensure the routes are added to the right Swagger doc
                c.DocInclusionPredicate((version, desc) =>
                {

                    if (!desc.TryGetMethodInfo(out MethodInfo methodInfo))
                        return false;

                    _logger.LogInformation("DocInclusionPredicate version=[" + version + "] method=[" + methodInfo.Module?.Name + "][" + methodInfo.Name + "]");

                    var versions = methodInfo.DeclaringType
                    .GetCustomAttributes(true)
                    .OfType<ApiExplorerSettingsAttribute>()
                    .Select(attr => attr.GroupName);

                    var maps = methodInfo
                    .GetCustomAttributes(true)
                    .OfType<ApiExplorerSettingsAttribute>()
                    .Select(attr => attr.GroupName)
                    .ToArray();


                    //_logger.LogInformation("DocInclusionPredicate versions.Any()=[" + versions.Any() + "] [" + String.Join(",", versions) + "] maps.Any()=[" + maps.Any() + "] [" + String.Join(",", maps) + "]");
                    if (versions.Any() || maps.Any())
                    {

                        var rt = maps.Any() && maps.Any(v => $"{v.ToString()}" == version) || !versions.Any() || versions.Any(v => $"{v.ToString()}" == version);
                        //_logger.LogInformation("DocInclusionPredicate rt=[" + rt + "]");
                        return rt;
                    }



                    var versions2 = methodInfo.DeclaringType
                    .GetCustomAttributes(true)
                    .OfType<ApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions);



                    //ApiExplorerSettings

                    var maps2 = methodInfo
                    .GetCustomAttributes(true)
                    .OfType<MapToApiVersionAttribute>()
                    .SelectMany(attr => attr.Versions);
                    //.ToArray();


                    //_logger.LogInformation("DocInclusionPredicate versions2.Any()=[" + versions2.Any() + "] [" + String.Join(",", versions2) + "] maps2.Any()=[" + maps2.Any() + "] [" + String.Join(",", maps2) + "]");
                    //_logger.LogInformation("DocInclusionPredicate versions2.Any()=[" + versions2.Any() + "] maps2.Any()=[" + maps2.Any() + "]");

                    return versions2.Any(v => $"v{v.ToString()}" == version)
                                  && (!maps2.Any() || maps2.Any(v => $"v{v.ToString()}" == version));
                });






                _logger.LogInformation("OperationFilter - AppendAuthorizeToSummaryOperationFilter");
                // DO NOT MODIFY :: Adds "(Auth)" to the summary so that you can see which endpoints have Authorization
                // or use the generic method, e.g. c.OperationFilter<AppendAuthorizeToSummaryOperationFilter<MyCustomAttribute>>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                foreach (var opt in options)
                {
                    addProfile(opt.Name, opt);
                }


                _logger.LogInformation("DocumentFilter - SwaggerFilterGen");
                c.DocumentFilter<SwaggerFilterGen>();


                _logger.LogInformation("Done");
            });

            //Register custom swagger example data objects here. Example data objects must be registered here to be used by swagger.
            //services.AddSwaggerExamplesFromAssemblyOf<DistributionLinksModelExample>();

            return services;
        }


    }
}
