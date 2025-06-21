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
        Task<ColourResponseDTO> GetColourByIdAsync(Guid id);
        Task<ColourResponseDTO> CreateColourAsync(ColourDTO colourDTO);
        Task<ColourResponseDTO> UpdateColourAsync(ColourDTO colourDTO);
        Task<ColourResponseDTO> DeleteColourAsync(Guid id);
    }
}
