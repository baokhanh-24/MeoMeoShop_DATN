using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Utilities
{
    public class ApiCaller : IApiCaller
    {
        private readonly HttpClient _http;
        private readonly LoadingService _loading;
        private readonly ILogger<ApiCaller> _logger;
        private readonly ProtectedLocalStorage _localStorage;
        private const string StorageKey = "accessToken";

        public ApiCaller(HttpClient http, LoadingService loading, ILogger<ApiCaller> logger,
            ProtectedLocalStorage localStorage)
        {
            _http = http;
            _loading = loading;
            _logger = logger;
            _localStorage = localStorage;
        }

        public async Task SetAuthorizeHeader()
        {
            try
            {
                var session = (await _localStorage.GetAsync<string>(StorageKey)).Value;
                if (session != null && !string.IsNullOrEmpty(session))
                {
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session);

                    var requestCulture = new RequestCulture(
                        CultureInfo.CurrentCulture,
                        CultureInfo.CurrentUICulture
                    );
                    var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                    _http.DefaultRequestHeaders.Add("Cookie",
                        $"{CookieRequestCultureProvider.DefaultCookieName}={cultureCookieValue}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default(T);
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<T?> PostAsync<TInput, T>(string url, TInput data)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default(T);
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<T?> PostFormAsync<T>(string url, MultipartFormDataContent formData)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var response = await _http.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] Status: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"[ERROR] Response body: {content}");
                    throw new HttpRequestException(
                        $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Body: {content}");
                }

                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default(T);
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<T?> PutAsync<TInput, T>(string url, TInput data)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default(T);
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<T?> PutFormAsync<T>(string url, MultipartFormDataContent formData)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var response = await _http.PutAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default(T);
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var response = await _http.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<string> GetRawAsync(string url)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return string.Empty;
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            await SetAuthorizeHeader();
            await _loading.StartAsync();
            try
            {
                var response = await _http.SendAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                await _loading.StopAsync();
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return [];
            }
            finally
            {
                await _loading.StopAsync();
            }
        }
    }
}