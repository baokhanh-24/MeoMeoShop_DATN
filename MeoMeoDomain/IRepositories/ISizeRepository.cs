using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ISizeRepository
    {
        Task<List<Size>> GetAllSize();
        Task<Size> GetSizeById(Guid id);
        Task Create(Size size);
        Task Update(Guid Id);
        Task Delete(Guid id);
    }
}
