using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IImageRepository
    {
        Task<List<Image>> GetAllImage();
        Task<Image> GetImageById(Guid id);
        Task Create(Image img);
        Task Update(Guid Id, Guid producDetailId);
        Task Delete(Guid id);
    }
}
