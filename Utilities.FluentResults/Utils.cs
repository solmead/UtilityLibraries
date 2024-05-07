using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Utilities.FluentResults.ApiModels;
using Utilities.FluentResults.Errors;

namespace Utilities.FluentResults
{
    public static class Utils
    {
        public static string FetchMetadataValueFromErrorCollection(Result result, string key)
        {
            string returnValue = string.Empty;
            // results.Errors.Select(x => x.Metadata).Select(y => y.Keys.Where(z => z.Equals("StoredProcedureName"))).FirstOrDefault();
            //var dictList = result.Errors.Select(x => x.Metadata);
            var dictList = FetchAllMetadataFromErrorCollection(result);
            var dict = dictList.Where(x => x.ContainsKey(key)).FirstOrDefault();
            if (dict != null)
            {
                returnValue = dict[key]?.ToString() ?? "";
            }

            return returnValue;
        }

        /// <summary>
        /// Returns a list of MetaData Dictioinaries
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> FetchAllMetadataFromErrorCollection(Result result)
        {
            if (result is null)
                return null;  //new IEnumerable<Dictionary<string, object>();


            // var dictList = result.Errors.Select(x => x.Metadata).Where(z => z.Count > 0);
            var errList = FetchAllErrorsThatContainMetadata(result);
            var mdlist = errList.Select(x => x.Metadata);
            //var dictList = result.Errors.Where(x => x.Metadata.Count() > 0).ToList();
            return mdlist;

        }

        public static IEnumerable<IError> FetchAllErrorsThatContainMetadata(Result result)
        {
            var errList = result.Errors.Where(x => x.Metadata.Count > 0);
            return errList;

        }


        public static ActionResult HandleFailedStatus(this Controller controller, Result result, int? statusCode = null)
        {

            var topLevelError = result.Errors[0];

            List<ApiError> apiErrors = new List<ApiError>();
            List<ApiErrorDetail> details = new List<ApiErrorDetail>();

            foreach (var err in result.Errors)
            {
                var errorTypeName = err.GetType().Name;
                apiErrors.Add(new ApiError
                {
                    ErrorType = errorTypeName,
                    Detail = new ApiErrorDetail(err.Message, err.Metadata)
                });
            }


            var errorResponse = new
            {
                message = "One or more errors occurred while processing your request",
                errors = apiErrors
            };

            if (topLevelError is RecordNotFoundError)
            {
                var idValue = FetchMetadataValueFromErrorCollection(result, "Id");
                var fieldName = FetchMetadataValueFromErrorCollection(result, "FieldName");

                var errorObject = new
                {
                    errorType = "RecordNotFoundError",
                    details
                };

                controller.HttpContext.Response.Headers.Add("x-response-error", "id value not found");
                controller.HttpContext.Response.Headers.Add("x-response-error-id", idValue.ToString());
                controller.HttpContext.Response.Headers.Add("x-response-error-field", fieldName);

                return controller.StatusCode(404, errorObject);
            }
            else if (topLevelError is MultiStepDependencyError)
            {
                return controller.StatusCode(422, errorResponse);
            }
            else if (topLevelError is InvalidRequestData)
            {
                return controller.StatusCode(422, errorResponse);
            }
            else if (topLevelError is SqlExceptionError)
            {
                return controller.StatusCode(500, errorResponse);
            }
            else if (topLevelError is StoredProcedureExecutionError)
            {
                return controller.StatusCode(500, errorResponse);
            }
            else if (topLevelError is DataTransformationError)
            {
                return controller.StatusCode(500, errorResponse);
            }
            else
            {
                return controller.StatusCode(500, errorResponse);
            }
        }



    }
}
