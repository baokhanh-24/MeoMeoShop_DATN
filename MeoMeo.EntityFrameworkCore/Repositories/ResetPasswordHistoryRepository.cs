using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ResetPasswordHistoryRepository : BaseRepository<ResetPasswordHistory>, IResetPasswordHistoryRepository
    {
        public ResetPasswordHistoryRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<ResetPasswordHistory> CreateResetPasswordHistoryAsync(ResetPasswordHistory resetPasswordHistory)
        {
            var resetPasswordHistoryAdded = await AddAsync(resetPasswordHistory);
            return resetPasswordHistoryAdded;
        }

        public async Task<bool> DeleteResetPasswordHistoryAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<ResetPasswordHistory>> GetAllResetPasswordHistoryAsync()
        {
            var getAllResetPasswordHistory = await GetAllAsync();
            return getAllResetPasswordHistory.ToList();
        }

        public async Task<ResetPasswordHistory> GetResetPasswordHistoryByIdAsync(Guid id)
        {
            var resetPasswordHistory = await GetByIdAsync(id);
            return resetPasswordHistory;
        }

        public async Task<ResetPasswordHistory> UpdateResetPasswordHistoryAsync(ResetPasswordHistory resetPasswordHistory)
        {
            var resetPasswordHistoryUpdated = await UpdateAsync(resetPasswordHistory);
            return resetPasswordHistoryUpdated;
        }
    }
}
