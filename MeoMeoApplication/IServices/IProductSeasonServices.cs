using MeoMeo.Contract.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductSeasonServices
    {
        Task<List<ProductSeasonDTO>> GetAllAsync();
        Task<ProductSeasonDTO> GetByIdAsync(Guid ProductId, Guid SeasonId);
        Task<ProductSeasonDTO> CreateAsync(ProductSeasonDTO dto);
        Task<ProductSeasonDTO> UpdateAsync(Guid ProductId, Guid Seasonid, ProductSeasonDTO dto);
        Task<bool> DeleteAsync(Guid ProductId, Guid SeasonId);
    }
}
