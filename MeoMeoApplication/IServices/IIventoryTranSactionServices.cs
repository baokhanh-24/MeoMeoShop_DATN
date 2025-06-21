using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IIventoryTranSactionServices
    {
        Task<IEnumerable<InventoryTransaction>> GetAllAsync();
        Task<CreateOrUpdateInventoryTranSactionResponse> GetByIdAsync(Guid id);
        Task<CreateOrUpdateInventoryTranSactionResponse> CreateAsync(InventoryTranSactionDTO dto);
        Task<CreateOrUpdateInventoryTranSactionResponse> UpdateAsync(Guid id, InventoryTranSactionDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
