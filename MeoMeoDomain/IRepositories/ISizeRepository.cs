using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ISizeRepository : IBaseRepository<Size>
    {
        public Task<IEnumerable<Size>> GetAllSize();
        public Task<Size> GetSizeById(Guid id);
        public Task<Size> Create(Size size);
        public Task<Size> Update(Size size);
        public Task<bool> Delete(Guid id);
    }
}
