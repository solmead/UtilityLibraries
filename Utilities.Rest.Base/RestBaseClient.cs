using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluentResults;
using Utilities.Poco;
using static System.Net.Mime.MediaTypeNames;

namespace Utilities.Rest.Base
{
    public abstract class RestBaseClient
    {

        protected readonly IHttpClientFactory _clientFactory;
        protected readonly ILogger _logger;
        protected readonly RestSettings _restSettings;

        public RestBaseClient(RestSettings restSettings, IHttpClientFactory httpClient,
                          ILogger logger)
        {
            _clientFactory = httpClient;
            _logger = logger;
            _restSettings = restSettings;
        }
        private Stopwatch sw = new Stopwatch();


        protected HttpClient _client
        {
            get
            {
                return _clientFactory.CreateClient(_restSettings.ClientName);
            }
        }

        protected void StartCall(string name)
        {
            sw = new Stopwatch();
            sw.Start();
            if (RestBaseConfigurator.useLoggingForClients)
            {
                _logger.LogDebug("calling " + name);
            }
        }

        protected string GetTimeElapsed()
        {
            return sw.Elapsed.ToString();
        }
        protected void EndCall(string name)
        {
            sw.Stop();
            if (RestBaseConfigurator.useLoggingForClients)
            {
                _logger.LogDebug("call " + name + " took [" + sw.Elapsed.ToString() + "]");
            }
        }

        private HttpContent getContent(object data, DataFormatEnum dataFormat, JsonSerializerOptions? options)
        {
            StringContent ret = new StringContent("");
            if (dataFormat == DataFormatEnum.JsonEncoded)
            {
                options = options ?? new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());

                var text = JsonSerializer.Serialize(data, options);
                ret = new StringContent(text, Encoding.UTF8, "application/json");
            }
            if (dataFormat == DataFormatEnum.FormEncoded)
            {
                var dic = data.PropertiesAsDictionary();
                var content = new FormUrlEncodedContent(dic);
                return content;
                //var memStream = new MemoryStream();
                //content.CopyToAsync(memStream).Wait();
                //byte[] dataBy = memStream.GetBuffer();
                //var str = Encoding.UTF8.GetString(dataBy);
                //ret = new StringContent(str, UnicodeEncoding.UTF8, "application/x-www-form-urlencoded");
            }


            return ret;
        }







        #region Get_Calls

        protected async Task<Result<TT?>> GetAsync<TT>(string url, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var response = await GetStringAsync(url, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;

                if (string.IsNullOrWhiteSpace(response_string))
                {
                    return Result.Ok(default(TT));
                }

                options = options ?? new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());
                var it = JsonSerializer.Deserialize<TT>(response_string, options);
                return Result.Ok(it);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.GetAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }

        protected async Task<Result<string>> GetStringAsync(string url, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            StartCall($"RestBaseClient -> GetStringAsync[{url}]");
            try
            {
                var client = _client;
                if (clientSetup != null)
                {
                    client = await clientSetup(client) ?? client;
                }
                var httpResponse = await client.GetAsync(url);
                var s = httpResponse.Content.ReadAsStringAsync().Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    return Result.Ok(s);
                }
                return Result.Fail(s);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString() + ", Inner Exception " + ex.InnerException?.ToString();
                var tme = GetTimeElapsed();
                _logger.LogError(ex, $"Error when calling RestBaseClient.GetStringAsync [{url}] Time:[{tme}] : {exceptionInfo}");
                return Result.Fail(exceptionInfo);
            }
            finally
            {
                EndCall($"RestBaseClient GetStringAsync");
            }
        }


        #endregion


        #region Put_Calls

        protected Task<Result> PutAsync(string url, object data, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            return PutAsync(url, data, DataFormatEnum.JsonEncoded, options, clientSetup);
        }

        protected Task<Result<TT?>> PutAsync<TT>(string url, object data, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            return PutAsync<TT>(url, data, DataFormatEnum.JsonEncoded, options, clientSetup);
        }

        protected async Task<Result> PutAsync(string url, object data, DataFormatEnum dataFormat, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var content = getContent(data, dataFormat, options);
                var response = await PutStringAsync(url, content, options, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.PutAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }
        protected async Task<Result<TT?>> PutAsync<TT>(string url, object data, DataFormatEnum dataFormat, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var content = getContent(data, dataFormat, options);
                var response = await PutStringAsync(url, content, options, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;

                if (string.IsNullOrWhiteSpace(response_string))
                {
                    return Result.Ok(default(TT));
                }

                options = options ?? new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());
                var it = JsonSerializer.Deserialize<TT>(response_string, options);
                return Result.Ok(it);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                var tme = GetTimeElapsed();
                //_logger.LogError(ex, $"Error when calling RestBaseClient.GetStringAsync [{url}] Time:[{tme}] : {exceptionInfo}");
                _logger.LogError(ex, $"Error when calling RestBaseClient.PutAsync [{url}] Time:[{tme}] : {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }

        protected async Task<Result<string>> PutStringAsync(string url, HttpContent? content = null, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            StartCall($"RestBaseClient -> PutStringAsync[{url}]");
            try
            {
                var client = _client;
                if (clientSetup != null)
                {
                    client = await clientSetup(client) ?? client;
                }

                var httpResponse = await client.PutAsync(url, content);
                var s = httpResponse.Content.ReadAsStringAsync().Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    return Result.Ok(s);
                }
                return Result.Fail(s);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString() + ", Inner Exception " + ex.InnerException?.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.PutStringAsync [{url}]: {exceptionInfo}");

                return Result.Fail(exceptionInfo);
            }
            finally
            {
                EndCall($"RestBaseClient PutStringAsync");
            }
        }

        #endregion

        #region Post_Calls

        protected Task<Result> PostAsync(string url, object data, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            return PostAsync(url, data, DataFormatEnum.JsonEncoded, options, clientSetup);
        }
        protected Task<Result<TT?>> PostAsync<TT>(string url, object data, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            return PostAsync<TT>(url, data, DataFormatEnum.JsonEncoded, options, clientSetup);
        }


        protected async Task<Result> PostAsync(string url, object data, DataFormatEnum dataFormat, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var content = getContent(data, dataFormat, options);
                var response = await PostStringAsync(url, content, options, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.PostAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }
        protected async Task<Result<TT?>> PostAsync<TT>(string url, object data, DataFormatEnum dataFormat, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var content = getContent(data, dataFormat, options);
                var response = await PostStringAsync(url, content, options, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;


                if (string.IsNullOrWhiteSpace(response_string))
                {
                    return Result.Ok(default(TT));
                }

                options = options ?? new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());
                var it = JsonSerializer.Deserialize<TT>(response_string, options);
                return Result.Ok(it);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.PostAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }


        protected async Task<Result<string>> PostStringAsync(string url, HttpContent? content = null, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            StartCall($"RestBaseClient -> PutStringAsync[{url}]");
            try
            {
                var client = _client;
                if (clientSetup != null)
                {
                    client = await clientSetup(client) ?? client;
                }
                var httpResponse = await client.PostAsync(url, content);

                var s = httpResponse.Content.ReadAsStringAsync().Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    return Result.Ok(s);
                }
                return Result.Fail(s);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString() + ", Inner Exception " + ex.InnerException?.ToString();
                var tme = GetTimeElapsed();
                //_logger.LogError(ex, $"Error when calling RestBaseClient.GetStringAsync [{url}] Time:[{tme}] : {exceptionInfo}");
                _logger.LogError(ex, $"Error when calling RestBaseClient.PutStringAsync [{url}] Time:[{tme}] : {exceptionInfo}");

                return Result.Fail(exceptionInfo);
            }
            finally
            {
                EndCall($"RestBaseClient PutStringAsync");
            }

        }


        #endregion



        #region Delete_Calls
        protected Task<Result<TT?>> DeleteAsync<TT>(string url, object data, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            return DeleteAsync<TT>(url, data, DataFormatEnum.JsonEncoded, options, clientSetup);
        }

        protected Task<Result> DeleteAsync(string url, object data, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            return DeleteAsync(url, data, DataFormatEnum.JsonEncoded, options, clientSetup);
        }


        protected async Task<Result<TT?>> DeleteAsync<TT>(string url, object data, DataFormatEnum dataFormat, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var content = getContent(data, dataFormat, options);

                var response = await DeleteStringAsync(url, content, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;


                if (string.IsNullOrWhiteSpace(response_string))
                {
                    return Result.Ok(default(TT));
                }

                options = options ?? new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());
                var it = JsonSerializer.Deserialize<TT>(response_string, options);
                return Result.Ok(it);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.DeleteAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }
        protected async Task<Result> DeleteAsync(string url, object data, DataFormatEnum dataFormat, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var content = getContent(data, dataFormat, options);

                var response = await DeleteStringAsync(url, content, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.DeleteAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }


        protected async Task<Result<TT?>> DeleteAsync<TT>(string url, JsonSerializerOptions? options = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var response = await DeleteStringAsync(url, null, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;


                if (string.IsNullOrWhiteSpace(response_string))
                {
                    return Result.Ok(default(TT));
                }

                options = options ?? new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                options.Converters.Add(new JsonStringEnumConverter());
                var it = JsonSerializer.Deserialize<TT>(response_string, options);
                return Result.Ok(it);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.DeleteAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }
        protected async Task<Result> DeleteAsync(string url, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            try
            {
                var response = await DeleteStringAsync(url, null, clientSetup);
                if (response.IsFailed)
                {
                    return Result.Fail(response.Reasons.First().Message);
                }
                var response_string = response.ValueOrDefault;

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString();// + ", Inner Exception " + ex.InnerException.ToString();
                _logger.LogError(ex, $"Error when calling RestBaseClient.DeleteAsync [{url}]: {exceptionInfo}");

                return Result.Fail($"Exception Occurred: {exceptionInfo}");
            }
        }


        protected async Task<Result<string>> DeleteStringAsync(string url, HttpContent? content = null, Func<HttpClient, Task<HttpClient>>? clientSetup = null)
        {
            StartCall($"RestBaseClient -> DeleteStringAsync[{url}]");
            try
            {
                var client = _client;
                if (clientSetup != null)
                {
                    client = await clientSetup(client) ?? client;
                }
                var httpResponse = await client.DeleteAsync(url, content);

                var s = httpResponse.Content.ReadAsStringAsync().Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    return Result.Ok(s);
                }
                return Result.Fail(s);
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToString() + ", Inner Exception " + ex.InnerException?.ToString();
                var tme = GetTimeElapsed();
                //_logger.LogError(ex, $"Error when calling RestBaseClient.GetStringAsync [{url}] Time:[{tme}] : {exceptionInfo}");
                _logger.LogError(ex, $"Error when calling RestBaseClient.DeleteStringAsync [{url}] Time:[{tme}] : {exceptionInfo}");

                return Result.Fail(exceptionInfo);
            }
            finally
            {
                EndCall($"RestBaseClient DeleteStringAsync");
            }
        }



        #endregion






    }
}
