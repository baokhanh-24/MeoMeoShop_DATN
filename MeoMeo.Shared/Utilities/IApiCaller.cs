namespace MeoMeo.Shared.Utilities;

public interface IApiCaller
{
    Task<T?> GetAsync<T>(string url);
    Task<string> GetRawAsync(string url);
    Task<T?> PostAsync<TInput, T>(string url, TInput data);
    Task<T?> PutAsync<TInput, T>(string url, TInput data);
    Task<bool> DeleteAsync(string url);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}