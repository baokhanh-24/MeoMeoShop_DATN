using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface ISizeService
    {
        Task<IEnumerable<Size>> GetAllSizeAsync();
        Task<Size> GetSizeByIdAsync(Guid id);
        Task<Size> CreateSizeAsync(SizeDTO sizeDTO);
        Task<Size> UpdateSizeAsync(SizeDTO sizeDTO);
        Task<bool> DeleteSizeAsync(Guid id);
    }
}
