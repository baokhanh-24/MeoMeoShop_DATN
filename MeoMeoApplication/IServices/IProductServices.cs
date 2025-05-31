using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IProductServices
    {
        Task<Product> GetProductAsync(Guid id);
        Task<Product> CreateProductAsync(CreateOrUpdateProductDTO product);
    }
}
