using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using Utilities.Swagger.Abstract;
using Utilities.Swagger.Configs;
using Utilities.Swagger.Configurator;
using Utilities.Swagger.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utilities.Swagger
{
    public class SwaggerFilterGen : IDocumentFilter, ISwaggerGen
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger _logger;
        public static List<string> EnumList = new List<string>();

        public static List<string> Objects = new List<string>();

        public Dictionary<string, ObjDefInfo> ObjectDic = new Dictionary<string, ObjDefInfo>();

        public SwaggerFilterGen(IWebHostEnvironment webHostEnvironment, ILogger logger)
        {
            //_fileHandler = new LocalFileHandler(new ServerFileServices());
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            _logger.LogInformation("SwaggerFilterGen - Apply");

            var profiles = SwaggerConfiguration.profileServices.Values.ToList();
            profiles = profiles ?? new List<SwaggerGenProfile>();

            if (!profiles.Any())
            {
                profiles.Add(new SwaggerStandardConfig());
            }

            foreach(var profile in profiles)
            {
                var generator = profile.GetFileGenerator(_logger, this);
                generator.StartSwaggerGen();

                WriteComponents(generator, swaggerDoc.Components);


                WriteNamespaces(generator, swaggerDoc.Paths);

                generator.EndSwaggerGen();

            }


            //data.AppendLine("");
            //data.AppendLine("");
            //data.AppendLine("");
            //WriteNamespaces(data, swaggerDoc.Paths);

            //data.AppendLine("//}");


            //_fileHandler.SaveFile(path, filename, dataArray);

            //throw new System.NotImplementedException();
            string jsonString = JsonSerializer.Serialize(swaggerDoc);

            //var a = 0;
            // swaggerDoc


            _logger.LogInformation("SwaggerFilterGen - Finished");

        }

        public string MapPath(string path)
        {
            //_logger.LogInformation("MapPath path=[" + path + "]");
            path = path.Trim();
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);

            var dirPath = "" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            var netPath = "" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;


            if (path.StartsWith(netPath) || path.Contains(dirPath))
            {
                //_logger.LogInformation("MapPath network or full path=[" + path + "]");
                return path;
            }

            path = path.Replace(netPath, "" + Path.DirectorySeparatorChar);


            if (path.First() == '~')
            {
                //_logger.LogInformation("MapPath starts with ~");
                path = path.Substring(1);
            }

            if (path.First() != Path.DirectorySeparatorChar)
            {
                path = Path.DirectorySeparatorChar + path;
            }


            //_logger.LogInformation("MapPath checking path=[" + path + "]");

            var pths = path.Split(Path.DirectorySeparatorChar);

            var subPath = "";
            for (var a = 2; a < pths.Length; a++)
            {
                subPath = subPath + Path.DirectorySeparatorChar + pths[a];
            }


            var finPath = "";

            string contentRootPath = _webHostEnvironment.ContentRootPath;
            //_logger.LogInformation("MapPath contentRootPath = [" + contentRootPath + "]");
            finPath = Path.GetFullPath(Path.Join(contentRootPath, path));

            return finPath;


            //path = path.Trim();
            //path = path.Replace("/", "\\");

            //if (path.StartsWith("\\\\") || path.Contains(":\\"))
            //{
            //    return path;
            //}

            //path = path.Replace("\\\\", "\\");


            //if (path.First() == '~')
            //{
            //    path = path.Substring(1);
            //}

            //if (path.First() != '\\')
            //{
            //    path = "\\" + path;
            //}


            ////string webRootPath = _webHostEnvironment.WebRootPath;
            //string contentRootPath = _webHostEnvironment.ContentRootPath;

            ////string path = "";
            ////path = Path.Combine(webRootPath, "CSS");



            //return Path.Join(contentRootPath, path);

            ////throw new NotImplementedException();
            ////throw new NotImplementedException();
        }

        public List<ObjParamInfo> GetAllParams((string Name, OpenApiPathItem Path) entry, (OperationType Name, OpenApiOperation Operation) operation, bool appendOperation = false)
        {
            var lst = new List<ObjParamInfo>();


            var last = GetFunctionName(entry, operation.Operation);

            if (appendOperation)
            {
                last = operation.Name.ToString() + last;
            }

            if (last.Contains("saveAttack"))
            {
                var a = 1;
            }

            OpenApiResponse response = null;
            if (operation.Operation.Responses.ContainsKey("200"))
            {
                response = operation.Operation.Responses["200"];
            }

            OpenApiMediaType jsonReturn = null;
            if (response?.Content.ContainsKey("application/json") ?? false)
            {
                jsonReturn = response?.Content["application/json"];
            }
            var funcType = HandleType(jsonReturn?.Schema);
            //operation.Operation.Parameters.First().In


            var requiredParams = (from p in operation.Operation.Parameters where p.Schema.Default == null || IsNullable(p.Schema) select p).ToList();
            var optionalParams = (from p in operation.Operation.Parameters where p.Schema.Default != null && !IsNullable(p.Schema) select p).ToList();


            var paramString = "";
            var paramCallString = "";
            foreach (var p in requiredParams)
            {
                lst.Add(new ObjParamInfo()
                {
                    Name =  p.Name,
                    DataType = HandleType(p.Schema),
                    IsNullable = IsNullable(p.Schema)
                });
            }
            var pname = "data";
            var hasBody = false;
            if (operation.Operation.RequestBody != null)
            {
                if (operation.Operation.RequestBody.Content.ContainsKey("application/json"))
                {
                    hasBody = true;
                    var content = operation.Operation.RequestBody.Content["application/json"];
                    var tpe = HandleType(content.Schema);

                    pname = GetBodyName(content.Schema);
                    if (string.IsNullOrWhiteSpace(pname))
                    {
                        pname = "data";
                    }

                    lst.Add(new ObjParamInfo()
                    {
                        Name = pname,
                        DataType = tpe,
                        IsNullable = true,
                        IsBody = true
                    });
                }
            }

            foreach (var p in optionalParams)
            {
                lst.Add(new ObjParamInfo()
                {
                    Name = p.Name,
                    DataType = HandleType(p.Schema),
                    IsNullable = IsNullable(p.Schema)
                });
            }
            if (!string.IsNullOrWhiteSpace(funcType))
            {
                lst.Add(new ObjParamInfo()
                {
                    Name = "returnValue",
                    DataType = funcType,
                    IsNullable = true,
                    IsReturned = true
                });
            }


            return lst;

        }
        public List<ObjParamInfo> GetAllParams((string Name, OpenApiPathItem Path) entry, bool appendOperation = false)
        {
            var lst = new List<ObjParamInfo>();


            var operations = (from p in entry.Path.Operations.Keys select (Name: p, Operation: entry.Path.Operations[p])).ToList();
            foreach (var op in operations)
            {
                var lst2 = GetAllParams(entry, op, operations.Count > 1 || appendOperation);
                lst.AddRange(lst2);
            }

            return lst;
        }
        private List<ObjParamInfo> GetAllParams(OpenApiPaths paths, string name)
        {
            var pList = (from p in paths.Keys select (Name: p, Path: paths[p])).ToList();

            var lst = new List<ObjParamInfo>();


            var Paths = (from p in pList
                         from op in p.Path.Operations.Values
                         from t in op.Tags
                         where t.Name == name
                         select p).Distinct().ToList();

            var PathList = (from p in pList
                            from op in p.Path.Operations.Values
                            from t in op.Tags
                            where t.Name == name
                            select GetFunctionName(p, op)).ToList();



            foreach (var p in Paths)
            {
                var fnames = (from op in p.Path.Operations.Values select GetFunctionName(p, op)).Distinct().ToList();
                var append = false;
                foreach (var f in fnames)
                {
                    append = append || PathList.Where(p => p == f).Count() > 1;
                }


                var l2 = GetAllParams(p, append);
                lst.AddRange(l2);
            }


            return lst;

        }

        public void WriteNamespaces(IFileGenerator generator, OpenApiPaths paths)
        {
            generator.StartNamespaces();

            var pList = (from p in paths.Keys select (Name: p, Path: paths[p])).ToList();



            var tags = (from p in pList from op in p.Path.Operations.Values from t in op.Tags select t.Name).Distinct().ToList().OrderBy(n => n);

            foreach (var name in tags)
            {
                var Paths = (from p in pList
                             from op in p.Path.Operations.Values
                             from t in op.Tags
                             where t.Name == name
                             select p).Distinct().ToList();

                var l1 = GetAllParams(paths, name);
                var l2 = l1.Select((p) => p.DataType).Distinct();

                var lst = l2.ToDictionary((p) => p, (p) => l1.First((p2) => p2.DataType == p));

                generator.StartNamespace(name, lst);


                var PathList = (from p in pList
                             from op in p.Path.Operations.Values
                             from t in op.Tags
                             where t.Name == name
                             select GetFunctionName(p, op)).ToList();

                

                foreach (var p in Paths)
                {
                    var fnames = (from op in p.Path.Operations.Values select GetFunctionName(p, op)).Distinct().ToList();
                    var append = false;
                    foreach(var f in fnames)
                    {
                        append = append || PathList.Where(p => p == f).Count() > 1;
                    }
                    

                    WriteRemoteCallBlock(name, generator, p, append);
                }
                generator.EndNamespace(name);
            }



            generator.EndNamespaces();
        }



        public void WriteRemoteCallBlock(string namespaceName, IFileGenerator generator, (string Name, OpenApiPathItem Path) entry, bool appendOperation = false)
        {


            var operations = (from p in entry.Path.Operations.Keys select (Name: p, Operation: entry.Path.Operations[p])).ToList();
            foreach (var op in operations)
            {
                WriteRemoteCall(namespaceName, generator, entry, op, operations.Count > 1 || appendOperation);
            }


        }

        public bool IsPrimitive(string dataType)
        {
            return dataType == "string" || dataType == "number" || dataType == "boolean" || dataType == "any";
        }

        public bool IsDefinedObject(string dataType)
        {
            return ObjectDic.ContainsKey(dataType);
        }

        public bool IsArray(string dataType)
        {
            return dataType.Contains("Array");
        }
        public bool IsGenericArray(string dataType)
        {
            return dataType.Contains("Array") && dataType.Contains("<");
        }
        public string ArraySubType(string dataType)
        {
            if (IsGenericArray(dataType)) {
                var st = dataType.IndexOf("<") + 1;
                var en = dataType.IndexOf(">") - 1;
                var subType = dataType.Substring(st, en - st + 1);
                return subType;
            }

            return "object";
        }

        public string MapDates(string dataType, string classPath = "", string prefillLines = "", int level = 1)
        {
            var sb = new StringBuilder();

            if (level > 5)
            {
                sb.AppendLine("//Too many levels deep");
                return sb.ToString();
            }
            if (ObjectDic.ContainsKey(dataType))
            {
                var od = ObjectDic[dataType];

                var sb2 = new StringBuilder();
                foreach (var param in od.Params.Keys)
                {
                    var tpe = od.Params[param].DataType;
                    if (tpe == "Date")
                    {
                        sb2.AppendLine(prefillLines + "     " + "if(" + classPath + param + "!=null) {");

                        sb2.AppendLine(prefillLines + "     " + "     " + classPath + param + " = new Date(" + classPath + param + ");");
                        sb2.AppendLine(prefillLines + "     " + "}");
                    }
                    else if (!IsPrimitive(tpe))
                    {
                        sb2.AppendLine(prefillLines + "     " + "if(" + classPath + param + "!=null) {");
                        sb2.Append(MapDates(tpe, classPath + param + ".", prefillLines + "          ", level + 1));
                        sb2.AppendLine(prefillLines + "     " + "}");
                    }
                }
                var sb2Str = sb2.ToString();
                if (!string.IsNullOrWhiteSpace(sb2Str))
                {
                    sb.AppendLine(prefillLines + "if(" + classPath.Substring(0, classPath.Length - 1) + "!=null) {");

                    sb.Append(sb2Str);

                    sb.AppendLine(prefillLines + "}");
                }
            }
            else if (dataType.Contains("Array") && dataType.Contains("<"))
            {
                var st = dataType.IndexOf("<") + 1;
                var en = dataType.IndexOf(">") - 1;
                var subType = dataType.Substring(st, en - st + 1);

                if (!IsPrimitive(subType))
                {
                    var mt = MapDates(subType, "item" + level + ".", prefillLines + "     " + "     ", level + 1);

                    if (!string.IsNullOrWhiteSpace(mt))
                    {
                        sb.AppendLine(prefillLines + "if(" + classPath.Substring(0, classPath.Length - 1) + "!=null) {");
                        sb.AppendLine(prefillLines + "     " + classPath + "forEach((item" + level + ") => {");

                        sb.Append(mt);

                        sb.AppendLine(prefillLines + "     " + "});");
                        sb.AppendLine(prefillLines + "}");
                    }
                }
            }

            return sb.ToString();
        }

        private string GetFunctionName((string Name, OpenApiPathItem Path) entry, OpenApiOperation operation)
        {
            var arr1 = entry.Name.Split("{");
            var arr2 = arr1[0].Split("/");

            var last = operation.OperationId;

            if (string.IsNullOrEmpty(last))
            {
                last = arr2[arr2.Length - 1];
            }

            if (string.IsNullOrEmpty(last) && arr2.Length >= 2)
            {
                last = arr2[arr2.Length - 2];
            }
            return last;
        }

        public void WriteRemoteCall(string namespaceName, IFileGenerator generator, (string Name, OpenApiPathItem Path) entry, (OperationType Name, OpenApiOperation Operation) operation, bool appendOperation = false)
        {
            var messages = new StringBuilder();
            messages.AppendLine("Namespace = " + namespaceName);
            messages.AppendLine("Operation = " + operation.Name.ToString());
            messages.AppendLine("appendOperation = " + appendOperation);
            messages.AppendLine("OrigUrl = " + entry.Name);

            var paramLst = GetAllParams(entry, operation, appendOperation).ToDictionary((p) => p.Name, (p) => p); ;

            //var url = entry.Name;
            var url = ("\"" + entry.Name.Replace("{", "\" + ").Replace("}", " + \"") + "\"");


            var funcName = GetFunctionName(entry, operation.Operation);

            if (appendOperation)
            {
                funcName = operation.Name.ToString() + funcName;
            }

            if (funcName.Contains("saveAttack"))
            {
                var a = 1;
            }


            var requiredParams = (from p in operation.Operation.Parameters where p.Schema.Default == null || IsNullable(p.Schema) select p).ToList();
            var optionalParams = (from p in operation.Operation.Parameters where p.Schema.Default != null && !IsNullable(p.Schema) select p).ToList();

            foreach (var p in requiredParams)
            {
                if (p.In == ParameterLocation.Query)
                {

                    url = url.Substring(0, url.Length-1) + (url.Contains("?") ? "&" : "?") + p.Name + "=\" + " + p.Name + " + \"\"";
                }

            }

            foreach (var p in optionalParams)
            {
                if (p.In == ParameterLocation.Query)
                {
                    url = url.Substring(0, url.Length-1) + (url.Contains("?") ? "&" : "?") + p.Name + "=\" + " + p.Name + " + \"\"";
                }
            }


            messages.AppendLine("FinalUrl = " + url);

            generator.WriteRemoteCall(namespaceName, operation.Name, funcName, paramLst, url, messages.ToString());

        }



        public void WriteComponents(IFileGenerator generator, OpenApiComponents components)
        {
            generator.StartComponents("All");
            foreach (var c in components.Schemas.Keys)
            {
                WriteSchema(generator, c, components.Schemas[c]);
            }
            generator.EndComponents("All");
        }

        public void WriteSchema(IFileGenerator generator, string key, OpenApiSchema schema)
        {
            var title = schema.Title ?? key;
            var type = GetTypeName(schema);

            generator.StartSchemas(title);

            //if (key.Contains("."))
            //{
            //    key = key.Substring(key.IndexOf(".") + 1);
            //}
            if (title.Contains("."))
            {
                title = title.Substring(title.LastIndexOf(".") + 1);
            }
            if (title.Contains("+"))
            {
                title = title.Substring(title.LastIndexOf("+") + 1);
            }


            if (key == "MeetingTime" || title == "BaseActor" || key.Contains("`"))
            {
                
                var a = 1;
            }

            Objects.Add(title);
            if (!IsEnum(schema))
            {
                var paramDic = new Dictionary<string, ObjParamInfo>();
                foreach (var paramName in schema.Properties.Keys)
                {
                    if (paramName== "MeetingTime" || paramName.Contains("`"))
                    {
                        var i = 0;
                    }
                    var param = schema.Properties[paramName];
                    if (IsValid(param))
                    {
                        var tpe = HandleType(param);
                        if (!paramDic.ContainsKey(paramName))
                        {
                            if(tpe == "TimeSpan" || tpe.Contains("`"))
                            {
                                var a = 1;
                            }

                            paramDic.Add(paramName, new ObjParamInfo()
                            {
                                Name = paramName,
                                DataType = tpe,
                                IsNullable = IsNullable(param)
                            });
                        }
                    }
                }

                if (!ObjectDic.ContainsKey(key))
                {
                    ObjectDic.Add(key, new ObjDefInfo() {
                         Params=paramDic,
                         Key = key,
                         Title = title
                    });
                }

                generator.WriteObjectDefinition(title, paramDic);
            }
            else
            {
                AddEnum(title);
                var lst = schema.Enum.Select((e) => (e as OpenApiString)?.Value ?? "").ToList();
                generator.WriteEnumDefinition(title, lst);
            }


            generator.EndSchemas(title);
        }


        public bool IsEnum(OpenApiSchema schema)
        {
            return schema.Enum?.Any() ?? false;
        }
        public bool IsNullable(OpenApiSchema schema)
        {
            return schema.Nullable;
        }
        public bool IsOptional(OpenApiSchema schema)
        {
            return schema.Default != null;
        }
        //public bool IsGeneric(OpenApiSchema schema)
        //{
        //    return schema.Gene;
        //}
        public bool IsValid(OpenApiSchema schema)
        {
            return true;
        }

        public string GetTypeName(OpenApiSchema schema)
        {



            var title = schema.Title;
            var type = schema.Format ?? schema.Type;
            type = type ?? "";

            if (type.ToUpper().Contains("DATE"))
            {
                //var a = 1;
            }

            if (string.IsNullOrWhiteSpace(type) && schema.Items?.Reference != null)
            {
                type = schema.Items.Reference.Id;
            }

            if (string.IsNullOrWhiteSpace(type) && schema.Reference != null)
            {
                type = schema.Reference.Id;
            }
            if (string.IsNullOrWhiteSpace(type) && (schema.AllOf?.Any() ?? false))
            {
                var sc = schema.AllOf[0];

                if (string.IsNullOrWhiteSpace(type) && sc.Items?.Reference != null)
                {
                    type = sc.Items.Reference.Id;
                }

                if (string.IsNullOrWhiteSpace(type) && sc.Reference != null)
                {
                    type = sc.Reference.Id;
                }
            }



            if (ObjectDic.ContainsKey(type))
            {
                var it = ObjectDic[type];
                type = it.Title;
            }

            if (type.Contains("."))
            {
                type = type.Substring(type.LastIndexOf(".") + 1);
            }
            if (type.Contains("+"))
            {
                type = type.Substring(type.LastIndexOf("+") + 1);
            }

            if (type.Contains("`"))
            {
                var ii = 0;
            }

            return type;
        }

        public string GetBodyName(OpenApiSchema schema)
        {
            //var type = schema.Format ?? schema.Type;
            var type = GetTypeName(schema);
           
            if (string.IsNullOrWhiteSpace(type))
            {
                type = "void";
            }
            //if (type.Contains("Nullable"))
            //{
            //    type = type.Replace("Nullable<", "").Replace(">", "");// + "?";
            //}

            var numbers = new List<string>
            {
                "integer",
                "int16",
                "int32",
                "int64",
                "uint16",
                "uint32",
                "uint64",
                "single",
                "double",
                "decimal",
                "byte",
                "sbyte"
            };
            numbers.ForEach((s) =>
            {
                type = type.ReplaceTypeName(s, "number");
            });
            type = type.ReplaceTypeName("Boolean", "boolean");
            //DateTime
            //type = type.Replace("DateTime", "string");
            //postal-code
            //email
            type = type.ReplaceTypeName("postal-code", "string");
            type = type.ReplaceTypeName("email", "string");
            type = type.ReplaceTypeName("tel", "string");
            type = type.ReplaceTypeName("uuid", "string");
            type = type.ReplaceTypeName("date-span", "string");

            type = type.ReplaceTypeName("date-time", "Date");
            type = type.ReplaceTypeName("date", "Date");
            type = type.ReplaceTypeName("timespan", "string");

            type = type.ReplaceTypeName("object", "any");
            type = type.ReplaceTypeName("fileinfo", "any");

            type = type.ReplaceTypeName("String", "string");
            type = type.ReplaceTypeName("char", "string");


            type = type.ReplaceTypeName("array", "Array");
            type = type.ReplaceTypeName("list<", "Array<");
            type = type.ReplaceTypeName("ilist", "Array");
            type = type.ReplaceTypeName("ienumerable", "Array");
            type = type.ReplaceTypeName("icollection", "Array");
            type = type.ReplaceTypeName("iqueryable", "Array");
            type = type.ReplaceTypeName("iarray", "Array");

            if (type == "Array")
            {
                type = HandleType(schema.Items) + "s";
            }

            var t2 = type.ToCamelCasing();


            
            return t2;
        }
        public string HandleType(OpenApiSchema schema)
        {
            if (schema == null)
            {
                return "any";
            }

            var type = GetTypeName(schema);

            if (string.IsNullOrWhiteSpace(type))
            {
                type = "void";
            }
            //if (type.Contains("Nullable"))
            //{
            //    type = type.Replace("Nullable<", "").Replace(">", "");// + "?";
            //}

            var numbers = new List<string>
            {
                "integer",
                "int16",
                "int32",
                "int64",
                "uint16",
                "uint32",
                "uint64",
                "single",
                "double",
                "decimal",
                "byte",
                "sbyte"
            };
            numbers.ForEach((s) =>
            {
                type = type.ReplaceTypeName(s, "number");
            });
            type = type.ReplaceTypeName("Boolean", "boolean");
            //DateTime
            //type = type.Replace("DateTime", "string");
            type = type.ReplaceTypeName("postal-code", "string");
            type = type.ReplaceTypeName("email", "string");
            type = type.ReplaceTypeName("tel", "string");
            type = type.ReplaceTypeName("uuid", "string");
            type = type.ReplaceTypeName("date-span", "string");

            type = type.ReplaceTypeName("date-time", "Date");
            type = type.ReplaceTypeName("date", "Date");
            type = type.ReplaceTypeName("timespan", "string");

            type = type.ReplaceTypeName("object", "any");
            type = type.ReplaceTypeName("fileinfo", "any");

            type = type.ReplaceTypeName("String", "string");
            type = type.ReplaceTypeName("char", "string");


            type = type.ReplaceTypeName("array", "Array");
            type = type.ReplaceTypeName("list<", "Array<");
            type = type.ReplaceTypeName("ilist", "Array");
            type = type.ReplaceTypeName("list", "Array");
            type = type.ReplaceTypeName("ienumerable", "Array");
            type = type.ReplaceTypeName("icollection", "Array");
            type = type.ReplaceTypeName("iqueryable", "Array");
            type = type.ReplaceTypeName("iarray", "Array");

            if (type == "Array")
            {
                type = type + "<" + HandleType(schema.Items) + ">";
            }


            return type;
        }

        public string DefaultValue(OpenApiSchema schema)
        {
            var type = HandleType(schema);
            if (IsNullable(schema))
            {
                return "null";
            }

            if (type.Contains("number"))
            {
                return "0";
            }
            if (type.Contains("boolean"))
            {
                return "false";
            }

            if (type.Contains("Date"))
            {
                return "new Date()";
            }


            return "null";
        }



        public string DefaultValue(string type)
        {

            type = type ?? "";

            if (IsArray(type))
            {
                var subType = ArraySubType(type);
                return "null";
            }




            if (type.Contains("number"))
            {
                return "0";
            }
            if (type.Contains("boolean"))
            {
                return "false";
            }

            if (type.Contains("Date"))
            {
                return "new Date()";
            }

            if (IsEnum(type))
            {
                return "0";
            }


            if (type.Contains("Nullable"))
            {
                return "null";
            }
            var numbers = new List<string>
            {
                "integer",
                "Int16",
                "Int32",
                "Int64",
                "UInt16",
                "UInt32",
                "UInt64",
                "Single",
                "Double",
                "Decimal",
                "Byte",
                "SByte"
            };
            foreach (var s in numbers)
            {
                if (type.Contains(s))
                {
                    return "0";
                }
            }

            if (type.Contains("Boolean"))
            {
                return "false";
            }

            if (type.Contains("DateTime"))
            {
                return "new Date()";
            }


            return "null";
        }
        public static bool IsValidOld(string type)
        {
            type = type ?? "";

            if (type.Contains("Func"))
            {
                return false;
            }
            return true;
        }
        public static bool IsNullableOld(string type)
        {
            type = type ?? "";
            return type.Contains("Nullable");
        }
        public void AddEnum(string type)
        {
            if (!EnumList.Contains(type))
            {
                EnumList.Add(type);
            }
        }
        public bool IsEnum(string type)
        {
            return EnumList.Contains(type);
        }

        public static bool IsBaseTypeOld(string type)
        {
            type = type ?? "";

            if (string.IsNullOrWhiteSpace(type))
            {
                return true;
            }

            if (type.Contains("Nullable"))
            {
                return true;
            }

            var numbers = new List<string>
            {
                "Int16",
                "Int32",
                "Int64",
                "UInt16",
                "UInt32",
                "UInt64",
                "Single",
                "Double",
                "Decimal",
                "Byte",
                "SByte"
            };
            foreach (var s in numbers)
            {
                if (type.Contains(s))
                {
                    return true;
                }
            }

            if (type.Contains("Boolean"))
            {
                return true;
            }

            if (type.Contains("DateTime"))
            {
                return true;
            }
            if (type.Contains("TimeSpan"))
            {
                return true;
            }
            if (type.Contains("Object"))
            {
                return true;
            }
            if (type.Contains("FileInfo"))
            {
                return true;
            }

            if (type.Contains("String"))
            {
                return true;
            }

            if (type.Contains("Char"))
            {
                return true;
            }


            return false;
        }
        public static string HandleTypeOld(string basetype)
        {
            var type = basetype;
            type = type ?? "";
            if (string.IsNullOrWhiteSpace(type))
            {
                type = "void";
            }
            if (type.Contains("Nullable"))
            {
                type = type.Replace("Nullable<", "").Replace(">", "");// + "?";
            }

            var numbers = new List<string>
            {
                "Int16",
                "Int32",
                "Int64",
                "UInt16",
                "UInt32",
                "UInt64",
                "Single",
                "Double",
                "Decimal",
                "Byte",
                "SByte"
            };
            numbers.ForEach((s) =>
            {
                type = type.Replace(s, "number");
            });
            type = type.Replace("Boolean", "boolean");
            //DateTime
            //type = type.Replace("DateTime", "string");
            type = type.Replace("DateTime", "Date");
            type = type.Replace("TimeSpan", "string");
            type = type.Replace("uuid", "string");

            type = type.Replace("Object", "any");
            type = type.Replace("FileInfo", "any");

            type = type.Replace("String", "string");
            type = type.Replace("Char", "string");


            type = type.Replace("List<", "Array<");
            type = type.Replace("IList", "Array");
            type = type.Replace("IEnumerable", "Array");
            type = type.Replace("ICollection", "Array");
            type = type.Replace("IQueryable", "Array");
            type = type.Replace("IArray", "Array");



            return type;
        }

        public bool IsDate(string type)
        {
            return type?.ToUpper() == "DATE";
        }
    }
}
