
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Utilities
{
    public class ApiCaller : IApiCaller
    {
        private readonly HttpClient _http;
        private readonly LoadingService _loading;
        private readonly ILogger<ApiCaller> _logger;

        public ApiCaller(HttpClient http, LoadingService loading, ILogger<ApiCaller> logger)
        {
            _http = http;
            _loading = loading;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<T?> PostAsync<TInput, T>(string url, TInput data)
        {
            await _loading.StartAsync();
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<T?> PostFormAsync<T>(string url, MultipartFormDataContent formData)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.PostAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<T?> PutAsync<TInput, T>(string url, TInput data)
        {
            await _loading.StartAsync();
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PutAsync(url, content);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<T?> PutFormAsync<T>(string url, MultipartFormDataContent formData)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.PutAsync(url, formData);
                response.EnsureSuccessStatusCode();
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(resultJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<string> GetRawAsync(string url)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            await _loading.StartAsync();
            try
            {
                var response = await _http.SendAsync(request);
                return response;
            }
            finally
            {
                 await _loading.StopAsync();
            }
        }
    }
}

