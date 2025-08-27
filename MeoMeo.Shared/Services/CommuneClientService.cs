using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.Services;

public class CommuneClientService : ICommuneClientService
{
    private readonly IApiCaller _api;

    public CommuneClientService(IApiCaller api)
    {
        _api = api;
    }

    public async Task<List<CommuneDTO>> GetAllAsync()
    {
        var data = await _api.GetAsync<List<CommuneDTO>>("/api/Communes");
        return data ?? new List<CommuneDTO>();
    }

    public async Task<List<CommuneDTO>> GetByDistrictIdAsync(Guid districtId)
    {
        var data = await _api.GetAsync<List<CommuneDTO>>($"/api/Communes/by-district/{districtId}");
        return data ?? new List<CommuneDTO>();
    }
}
