using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.IServices;

public interface IDistrictClientService
{
    Task<List<DistrictDTO>> GetAllAsync();
    Task<List<DistrictDTO>> GetByProvinceIdAsync(Guid provinceId);
}
