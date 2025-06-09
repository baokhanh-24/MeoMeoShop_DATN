using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductDetailServices
    {
        Task<IEnumerable<ProductDetail>> GetProductDetails();
        Task<ProductDetail?> GetByIdAsync(Guid id);
        Task AddAsync(ProductDetail entity);
        Task UpdateAsync(ProductDetail entity);
        Task DeleteAsync(Guid id);
        
    }
}
