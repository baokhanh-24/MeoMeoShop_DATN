using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProductsDetailRepository : BaseRepository<ProductDetail>, IProductsDetailRepository
    {


        public ProductsDetailRepository(MeoMeoDbContext context) : base(context)
        {

        }

        public async Task<ProductDetail> AddProductAsync(ProductDetail entity)
        {
            var product = await AddAsync(entity);
            return product;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ProductDetail>> GetAllProductAsync()
        {
            return await GetAllAsync();
        }

        public async Task<ProductDetail> GetByProductIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<ProductDetail> UpdateProductAsync(ProductDetail entity)
        {
            var product = await UpdateAsync(entity);
            return product;
        }
    }
}
       