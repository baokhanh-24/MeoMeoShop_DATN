using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductDetaillSizeRepository
    {
        Task<List<ProductDetailSize>> GetAllProductDetaillSize();
        Task<ProductDetailSize> GetProductDetaillSizeById(Guid id);
        Task Create(ProductDetailSize productDetailSize);
        Task Update(Guid Id, Guid ProductDetaillId);
        Task Delete(Guid id);
    }
}
