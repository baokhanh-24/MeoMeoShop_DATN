using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories;

public interface IUserTokenRepository:IBaseRepository<UserToken>
{
    Task<UserToken> SaveAccessTokenAsync(UserToken userToken);
    Task UpdateRevokeAllOldTokenAsync(Guid UserId);
    Task<bool> RevokeTokenAsync(Guid token);
}