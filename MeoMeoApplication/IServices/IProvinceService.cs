using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IProvinceService
    {
        Task<IEnumerable<Province>> GetAllProvinceAsync();
        Task<Province> GetProvinceByIdAsync(Guid id);
        Task<Province> CreateProvinceAsync(CreateOrUpdateProvinceDTO province);
        Task<Province> UpdateProvinceAsync(CreateOrUpdateProvinceDTO province);
        Task<bool> DeleteProvinceAsync(Guid id);
    }
}
