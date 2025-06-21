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
    public class ImageRepository : BaseRepository<Image>, IImageRepository
    {
        public ImageRepository(MeoMeoDbContext context) : base(context)
        {
            
        }

        public async Task<Image> CreateImage(Image img)
        {
            var itemCreate = await AddAsync(img);
            return itemCreate;
        }

        public async Task<bool> DeleteImage(Guid id)
        {
            
            await DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Image>> GetAllImage()
        {
            var itemGetAll = await GetAllAsync();
            return itemGetAll.ToList();
        }

        public async Task<Image> GetImageById(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Image> UpdateImage(Image img)
        {
            var itemUpdate = await UpdateAsync(img);
            return itemUpdate;
        }
    }
}
