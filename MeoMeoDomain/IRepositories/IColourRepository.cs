using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IColourRepository : IBaseRepository<Colour>
    {
        public Task<IEnumerable<Colour>> GetAllColour();
        public Task<Colour> GetColourById(Guid id);
        public Task<Colour> Create(Colour colour);
        public Task<Colour> Update(Colour colour);
        public Task<bool> Delete(Guid id);
    }
}
