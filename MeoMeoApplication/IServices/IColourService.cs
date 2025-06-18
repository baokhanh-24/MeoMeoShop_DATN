using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IColourService
    {
        Task<IEnumerable<Colour>> GetAllColoursAsync();
        Task<Colour> GetColourByIdAsync(Guid id);
        Task<Colour> CreateColourAsync(ColourDTO colourDTO);
        Task<Colour> UpdateColourAsync(ColourDTO colourDTO);
        Task<bool> DeleteColourAsync(Guid id);
    }
}
