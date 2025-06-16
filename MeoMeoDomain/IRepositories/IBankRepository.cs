using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IBankRepository
    {
        Task<Bank> CreateBankAsync(Bank bank);
        Task<List<Bank>> GetAllBankAsync();
        Task<Bank> GetBankByIdAsync(Guid id);
        Task<Bank> UpdateBankAsync(Bank bank);
        Task<bool> DeleteBankAsync(Guid id);
    }
}
