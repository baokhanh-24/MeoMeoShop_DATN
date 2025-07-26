using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IRefreshTokenRepository
{
    Task<UserToken> FindRefreshTokenByTokenAsync(string token);
}