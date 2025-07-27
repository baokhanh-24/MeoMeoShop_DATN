using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class UserTokenRepository : BaseRepository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<UserToken> SaveAccessTokenAsync(UserToken userToken)
        {
            await AddAsync(userToken);
            return userToken;
        }

        public async Task UpdateRevokeAllOldTokenAsync(Guid UserId)
        {
            var oldTokens = await _context.userTokens
                .Where(x => x.UserId == UserId && !x.IsRevoked)
                .ToListAsync();

            foreach (var token in oldTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> RevokeTokenAsync(Guid token)
        {
            var userToken = await _context.userTokens
                .FirstOrDefaultAsync(x => x.RefreshToken == token.ToString());

            if (userToken != null)
            {
                userToken.IsRevoked = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
} 