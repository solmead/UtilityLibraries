using FluentResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities.PdfHandling.Models;
using Utilities.PdfHandling.NetCore.Abstracts;

namespace Utilities.PdfHandling.NetCore.Concretes
{
    public class PdfServicesClient : IPdfServicesClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<PdfServicesClient> _logger;

        public PdfServicesClient(IHttpClientFactory httpClient,
                          ILogger<PdfServicesClient> logger = null)
        {
            _clientFactory = httpClient;
            _logger = logger;
        }
        private Stopwatch sw = new Stopwatch();

        private HttpClient _client
        {
            get
            {
                var clientName = "PdfServices" + Core.config.CurrentServer.ToString() + "Client";
                _logger.LogInformation("PdfServicesClient Looking for client [" + clientName + "]");

                var c = _clientFactory.CreateClient(clientName);


                return c;
                //return null;
            }
        }
        private void StartCall(string name)
        {
            _logger.LogDebug("calling " + name);
            sw = new Stopwatch();
            sw.Start();
        }

        private void EndCall(string name)
        {
            sw.Stop();
            _logger.LogDebug("call " + name + " took [" + sw.Elapsed.ToString() + "]");
        }
        public async Task<Result<FileEntry>> GetPdfFromUrlAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            _logger.LogInformation("PdfServicesClient GetPdfFromUrlAsync [" + url + "]");
            try
            {
                var callResponse = await GetPdfFromUrlStringAsync(url, orientation);
                if (callResponse.IsFailed)
                {
                    return Result.Fail("GetPdfFromUrlAsync Request failed: " + callResponse.Reasons.FirstOrDefault()?.Message);
                }
                var response = callResponse.ValueOrDefault;

                if (string.IsNullOrWhiteSpace(response))
                {
                    return Result.Fail("GetPdfFromUrlAsync Request failed: " + "Nothing returned");
                }
                response = response.Trim();
                //if (response.ToLower().Contains("<error>") || response.ToUpper().Contains("RESOURCE NOT FOUND"))
                //{
                //    return Result.Fail("Item not found");
                //}
                //var settings = new JsonSerializerSettings { DateFormatString = "MM/dd/yyyy HH:mm" };

                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter());
                //options.Converters.Add(new CustomDateTimeConverter());
                var file = JsonSerializer.Deserialize<FileEntry>(response, options);
                return Result.Ok(file);
            }
            catch (Exception ex)
            {
                //ToDo:  Result Class return here with Failure code.
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling GetPdfFromUrlAsync: {exceptionInfo}");
                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }
        private async Task<Result<string>> GetPdfFromUrlStringAsync(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            StartCall($"PdfServiceClient -> GetPdfFromUrlStringAsync");
            try
            {

                var client = _client;
                _logger.LogInformation($"PdfServiceClient -> GetPdfFromUrlStringAsync -> Creating request");
                var dt = new PdfCreateRequest()
                {
                    Url = url,
                    Orientation = orientation
                };


                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter());

                var text = JsonSerializer.Serialize(dt, options);
                var content = new StringContent(text, Encoding.UTF8, "application/json");

                _logger.LogInformation("GetPdfFromUrlStringAsync remote call to - [" + _client.BaseAddress.ToString() + "] [api/v1/Pdf/GetPdfFromUrl]");
                var httpResponse = await client.PostAsync("api/v1/Pdf/GetPdfFromUrl", content);
                if (httpResponse.IsSuccessStatusCode)
                {
                    return Result.Ok(httpResponse.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return Result.Fail(httpResponse.StatusCode + " - " + httpResponse.ReasonPhrase + " - [" + httpResponse.Content.ReadAsStringAsync().Result + "]");
                }
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString() + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling GetPdfFromUrlStringAsync: {exceptionInfo}");

                return Result.Fail(exceptionInfo);
            }
            finally
            {
                EndCall($"PdfServiceClient -> GetPdfFromUrlStringAsync");
            }
        }

        public Result<FileEntry> GetPdfFromUrl(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            try
            {
                var callResponse = GetPdfFromUrlString(url, orientation);
                if (callResponse.IsFailed)
                {
                    return Result.Fail(callResponse.Reasons.FirstOrDefault()?.Message);
                }
                var response = callResponse.ValueOrDefault;

                if (string.IsNullOrWhiteSpace(response))
                {
                    return Result.Fail("Nothing returned");
                }
                response = response.Trim();
                //if (response.ToLower().Contains("<error>") || response.ToUpper().Contains("RESOURCE NOT FOUND"))
                //{
                //    return Result.Fail("Item not found");
                //}
                //var settings = new JsonSerializerSettings { DateFormatString = "MM/dd/yyyy HH:mm" };

                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter());
                //options.Converters.Add(new CustomDateTimeConverter());
                var file = JsonSerializer.Deserialize<FileEntry>(response, options);
                return Result.Ok(file);
            }
            catch (Exception ex)
            {
                //ToDo:  Result Class return here with Failure code.
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling GetPdfFromUrl: {exceptionInfo}");
                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }



        private Result<string> GetPdfFromUrlString(string url, PageOrientation orientation = PageOrientation.Portrait)
        {
            StartCall($"PdfServiceClient -> GetPdfFromUrlString");
            try
            {

                var client = _client;
                _logger.LogInformation($"PdfServiceClient -> GetPdfFromUrlString -> Creating request");

                var dt = new PdfCreateRequest()
                {
                    Url = url,
                    Orientation = orientation
                };


                var options = new JsonSerializerOptions();
                options.Converters.Add(new JsonStringEnumConverter());

                var text = JsonSerializer.Serialize(dt, options);
                var content = new StringContent(text, Encoding.UTF8, "application/json");

                _logger.LogInformation("GetPdfFromUrlString remote call to - [" + _client.BaseAddress.ToString() + "]");
                var t = client.PostAsync("api/v1/Pdf/GetPdfFromUrl", content);
                t.Wait();

                var httpResponse = t.Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    return Result.Ok(httpResponse.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    return Result.Fail("GetPdfFromUrlString Request failed: " + httpResponse.StatusCode + " - " + httpResponse.ReasonPhrase + " - [" + httpResponse.Content.ReadAsStringAsync().Result + "]");
                }
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString() + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling GetPdfFromUrlString: {exceptionInfo}");

                return Result.Fail(exceptionInfo);
            }
            finally
            {
                EndCall($"PdfServiceClient -> GetPdfFromUrlString");
            }
        }
    }
}
