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
    public class ProductDetaillColourRepository : BaseRepository<ProductDetailColour>, IProductDetaillColourRepository
    {
        public ProductDetaillColourRepository(MeoMeoDbContext context) : base(context) 
        {
            
        }
        public async Task<ProductDetailColour> Create(ProductDetailColour productDetailColour)
        {
            var itemCreate = await AddAsync(productDetailColour);
            return itemCreate;
        }

        public async Task<bool> Delete(Guid id)
        {           
            await DeleteAsync (id);
            return true;
        }

        public async Task<IEnumerable<ProductDetailColour>> GetAllProductDetaillColour()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }

        public async Task<ProductDetailColour> GetProductDetaillColourById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<ProductDetailColour> Update(ProductDetailColour productDetailColour)
        {
            var itemUpdate = await UpdateAsync(productDetailColour);
            return itemUpdate;
        }
    }
}
