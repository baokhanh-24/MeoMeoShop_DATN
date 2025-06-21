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
      
        public Task<IEnumerable<ProductDetail>> GetProductDetailAllAsync();
        public Task<ProductDetail> GetProductDetailByIdAsync(Guid id);
        public Task<ProductDetail> AddProductDetailAsync(CreateOrUpdateProductDetailDTO dto);
        public Task<ProductDetailResponseDTO> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO entity);
        public Task<ProductDetailResponseDTO> DeleteProductDetailAsync(Guid id);
       
    }
}
