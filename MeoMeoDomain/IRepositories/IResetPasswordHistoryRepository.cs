using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IResetPasswordHistoryRepository: IBaseRepository<ResetPasswordHistory>
    {
        Task<ResetPasswordHistory> CreateResetPasswordHistoryAsync(ResetPasswordHistory resetPasswordHistory);
        Task<List<ResetPasswordHistory>> GetAllResetPasswordHistoryAsync();
        Task<ResetPasswordHistory> GetResetPasswordHistoryByIdAsync(Guid id);
        Task<ResetPasswordHistory> UpdateResetPasswordHistoryAsync(ResetPasswordHistory resetPasswordHistory);
        Task<bool> DeleteResetPasswordHistoryAsync(Guid id);
    }
}
