using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductDetaillColourRepository
    {
        Task<List<ProductDetailColour>> GetAllProductDetaillColour();
        Task<ProductDetailColour> GetProductDetaillColourById(Guid id);
        Task Create(ProductDetailColour productDetailColour);
        Task Update(Guid Id, Guid ProductDetaillId);
        Task Delete(Guid id);
    }
}
