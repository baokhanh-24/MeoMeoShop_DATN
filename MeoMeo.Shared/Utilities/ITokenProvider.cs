namespace MeoMeo.Shared.Utilities;

public interface ITokenProvider
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SetAccessTokenAsync(string token);
    Task SetRefreshTokenAsync(string token);
    Task ClearAsync();
}
