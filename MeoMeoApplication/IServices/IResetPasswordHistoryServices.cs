using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IResetPasswordHistoryServices
    {
        Task<List<ResetPasswordHistory>> GetAllResetPasswordHistoryAsync();
        Task<ResetPasswordHistory> GetResetPasswordHistoryByIdAsync(Guid id);
        Task<ResetPasswordHistory> CreateResetPasswordHistoryAsync(CreateOrUpdateResetPasswordHistoryDTO resetPasswordHistory);
        Task<ResetPasswordHistory> UpdateResetPasswordHistoryAsync(CreateOrUpdateResetPasswordHistoryDTO resetPasswordHistory);
        Task<bool> DeleteResetPasswordHistoryAsync(Guid id);
    }
}
