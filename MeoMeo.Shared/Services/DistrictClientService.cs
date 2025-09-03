using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.Services;

public class DistrictClientService : IDistrictClientService
{
    private readonly IApiCaller _api;

    public DistrictClientService(IApiCaller api)
    {
        _api = api;
    }

    public async Task<List<DistrictDTO>> GetAllAsync()
    {
        var data = await _api.GetAsync<List<DistrictDTO>>("/api/Districts");
        return data ?? new List<DistrictDTO>();
    }

    public async Task<List<DistrictDTO>> GetByProvinceIdAsync(Guid provinceId)
    {
        var data = await _api.GetAsync<List<DistrictDTO>>($"/api/Districts/by-province/{provinceId}");
        return data ?? new List<DistrictDTO>();
    }
}
