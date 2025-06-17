using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IProductDetaillColourRepository : IBaseRepository<ProductDetailColour>
    {
        public Task<IEnumerable<ProductDetailColour>> GetAllProductDetaillColour();
        public Task<ProductDetailColour> GetProductDetaillColourById(Guid id);
        public Task<ProductDetailColour> Create(ProductDetailColour productDetailColour);
        public Task<ProductDetailColour> Update(ProductDetailColour productDetailColour);
        public Task<bool> Delete(Guid id);
    }
}
