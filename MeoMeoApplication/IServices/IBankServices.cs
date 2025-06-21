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
        Task<CreateOrUpdateBankResponseDTO> GetBankByIdAsync(Guid id);
        Task<Bank> CreateBankAsync(CreateOrUpdateBankDTO bank);
        Task<CreateOrUpdateBankResponseDTO> UpdateBankAsync(CreateOrUpdateBankDTO bank);
        Task<bool> DeleteBankAsync(Guid id);
    }
}
