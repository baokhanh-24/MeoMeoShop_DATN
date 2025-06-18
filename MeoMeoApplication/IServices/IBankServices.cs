using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IBankServices
    {
        Task<List<Bank>> GetAllBankAsync();
        Task<Bank> GetBankByIdAsync(Guid id);
        Task<Bank> CreateBankAsync(CreateOrUpdateBankDTO bank);
        Task<Bank> UpdateBankAsync(CreateOrUpdateBankDTO bank);
        Task<bool> DeleteBankAsync(Guid id);
    }
}
