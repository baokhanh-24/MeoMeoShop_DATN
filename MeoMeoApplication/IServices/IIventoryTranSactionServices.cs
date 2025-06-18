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
        Task<InventoryTransaction> GetByIdAsync(Guid id);
        Task<InventoryTransaction> CreateAsync(InventoryTranSactionDTO dto);
        Task<InventoryTransaction> UpdateAsync(Guid id, InventoryTranSactionDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
