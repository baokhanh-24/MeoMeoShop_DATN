using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Shared.IServices
{
    public interface IColourClientService
    {
        Task<IEnumerable<Colour>> GetAllColoursAsync();
        Task<ColourResponseDTO> GetColourByIdAsync(Guid id);
        Task<ColourResponseDTO> CreateColourAsync(ColourDTO colourDTO);
        Task<ColourResponseDTO> UpdateColourAsync(ColourDTO colourDTO);
        Task<ColourResponseDTO> DeleteColourAsync(Guid id);
    }
} 