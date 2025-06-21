using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class ProductDetaillSizeRepository : BaseRepository<ProductDetailSize>, IProductDetaillSizeRepository
    {
        public ProductDetaillSizeRepository(MeoMeoDbContext context) : base(context)
        {
            
        }
        public async Task<ProductDetailSize> Create(ProductDetailSize productDetailSize)
        {
            var itemCreate = await AddAsync(productDetailSize);
            return itemCreate;
        }

        public async Task<bool> Delete(Guid id)
        {       
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ProductDetailSize>> GetAllProductDetaillSize()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }

        public async Task<ProductDetailSize> GetProductDetaillSizeById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<ProductDetailSize> Update(ProductDetailSize productDetailSize)
        {
            var itemUpdate = await UpdateAsync(productDetailSize);
            return itemUpdate;
        }
    }
}
