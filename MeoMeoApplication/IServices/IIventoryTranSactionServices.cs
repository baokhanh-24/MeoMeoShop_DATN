using MeoMeo.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IIventoryTranSactionServices
    {
        Task<List<InventoryTranSactionDTO>> GetAllAsync();
        Task<InventoryTranSactionDTO> GetByIdAsync(Guid id);
        Task<InventoryTranSactionDTO> CreateAsync(InventoryTranSactionDTO dto);
        Task<InventoryTranSactionDTO> UpdateAsync(Guid id, InventoryTranSactionDTO dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
