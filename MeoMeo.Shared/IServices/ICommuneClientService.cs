using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.IServices;

public interface ICommuneClientService
{
    Task<List<CommuneDTO>> GetAllAsync();
    Task<List<CommuneDTO>> GetByDistrictIdAsync(Guid districtId);
}
