using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductCategoryRepository : IBaseRepository<ProductCategory>
    {
        Task<IEnumerable<ProductCategory>> GetByProductIdAsync(Guid productId);
        Task DeleteByProductIdAsync(Guid productId);
    }
} 