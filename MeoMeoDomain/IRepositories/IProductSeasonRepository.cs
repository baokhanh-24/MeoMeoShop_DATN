using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductSeasonRepository
    {
        Task<List<ProductSeason>> GetAllAsync();
        Task<ProductSeason> GetByIdAsync(Guid id);
        Task<ProductSeason> CreateAsync(ProductSeason entity);
        Task<ProductSeason> UpdateAsync(Guid id, ProductSeason entity);
        Task<bool> DeleteAsync(Guid id);
    }
}
