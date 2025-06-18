using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductSeasonRepository : IBaseRepository<ProductSeason>
    {
        Task<IEnumerable<ProductSeason>> GeProductSeasontAllAsync();
        Task<ProductSeason> GetProductSeasonByIdAsync(Guid ProductId, Guid SeasonId);
        Task<ProductSeason> CreateProductSeasonAsync(ProductSeason productSeason);
        Task<ProductSeason> UpdateProductSeasonAsync(ProductSeason productSeason);
        Task<bool> DeleteProductSeasonAsync(Guid ProductId, Guid SeasonId);
        void RemoveProductSeason(ProductSeason productSeason);
    }
}
