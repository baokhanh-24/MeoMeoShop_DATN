using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IImageService
    {
        Task<IEnumerable<Image>> GetAllImagesAsync();
        Task<ImageResponseDTO> GetImageByIdAsync(Guid id);
        Task<ImageResponseDTO> CreateImageAsync(ImageDTO imageDto);
        Task<ImageResponseDTO> UpdateImageAsync(ImageDTO imageDto);
        Task<ImageResponseDTO> DeleteImageAsync(Guid id);
    }
}
