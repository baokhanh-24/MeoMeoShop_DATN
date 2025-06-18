using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductSeasonServices
    {
        Task<IEnumerable<ProductSeasonDTO>> GetAllProductSeasonAsync();
        Task<ProductSeason> GetProductSeasonByIdAsync(Guid ProductId, Guid SeasonId);
        Task<ProductSeason> CreateProductSeasonAsync(ProductSeasonDTO dto);
        Task<ProductSeason> UpdateProductSeasonAsync(Guid ProductId, Guid Seasonid, ProductSeasonDTO dto);
        Task<bool> DeleteProductSeasonAsync(Guid ProductId, Guid SeasonId);
    }
}
