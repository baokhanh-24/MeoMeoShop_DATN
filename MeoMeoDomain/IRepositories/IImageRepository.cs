using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IImageRepository : IBaseRepository<Image>
    {
        public Task<IEnumerable<Image>> GetAllImage();
        public Task<Image> GetImageById(Guid id);
        public Task<Image> CreateImage(Image img);
        public Task<Image> UpdateImage(Image img);
        public Task<bool> DeleteImage(Guid id);
    }
}
