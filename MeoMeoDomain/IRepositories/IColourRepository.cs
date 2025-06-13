using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface IColourRepository
    {
        Task<List<Colour>> GetAllColour();
        Task<Colour> GetColourById(Guid id);
        Task Create(Colour colour);
        Task Update(Guid Id);
        Task Delete(Guid id);
    }
}
