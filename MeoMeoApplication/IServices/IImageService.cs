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
        Task<Image> GetImageByIdAsync(Guid id);
        Task<Image> CreateImageAsync(ImageDTO imageDto);
        Task<Image> UpdateImageAsync(ImageDTO imageDto);
        Task<bool> DeleteImageAsync(Guid id);
    }
}
