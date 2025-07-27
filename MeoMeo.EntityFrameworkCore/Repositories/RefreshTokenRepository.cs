using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MeoMeoDbContext _context;

        public RefreshTokenRepository(MeoMeoDbContext context)
        {
            _context = context;
        }

        public async Task<UserToken> FindRefreshTokenByTokenAsync(string token)
        {
            return await _context.userTokens
                .FirstOrDefaultAsync(x => x.RefreshToken == token);
        }
    }
} 