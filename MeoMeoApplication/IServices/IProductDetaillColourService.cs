using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductDetaillColourService
    {
        Task<IEnumerable<ProductDetailColour>> GetAllProductDetaillColourAsync();
        Task<ProductDetailColour> GetProductDetaillColourByIdAsync(Guid id);
        Task<ProductDetailColour> CreateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO);
        Task<ProductDetailColour> UpdateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO);
        Task<bool> DeleteProduuctDetaillColourAsync(Guid id);
    }
}
