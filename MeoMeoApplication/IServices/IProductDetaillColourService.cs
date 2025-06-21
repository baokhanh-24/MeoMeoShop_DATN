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
        Task<ProductDetaillColourResponseDTO> GetProductDetaillColourByIdAsync(Guid id);
        Task<ProductDetaillColourResponseDTO> CreateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO);
        Task<ProductDetaillColourResponseDTO> UpdateProductDetaillColourAsync(ProductDetaillColourDTO productDetaillColourDTO);
        Task<ProductDetaillColourResponseDTO> DeleteProduuctDetaillColourAsync(Guid id);
    }
}
