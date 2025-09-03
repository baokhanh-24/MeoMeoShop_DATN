using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.IServices;

public interface IProvinceClientService
{
    Task<List<ProvinceDTO>> GetAllAsync();
}
