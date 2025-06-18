using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductDetaillSizeService
    {
        Task<IEnumerable<ProductDetailSize>> GetAllProductDetaillSizeAsync();
        Task<ProductDetailSize> GetProductDetaillSizeByIdAsync(Guid id);
        Task<ProductDetailSize> CreateProductDetaillSizeAsync(ProductDetaillSizeDTO productDetaillSizeDTO);
        Task<ProductDetailSize> UpdateProductDetaillSizeAsync(ProductDetaillSizeDTO productDetaillSizeDTO);
        Task<bool> DeleteProductDetaillSizeAsync(Guid id);
    }
}
