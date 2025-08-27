using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.Services;

public class ProvinceClientService : IProvinceClientService
{
    private readonly IApiCaller _api;

    public ProvinceClientService(IApiCaller api)
    {
        _api = api;
    }

    public async Task<List<ProvinceDTO>> GetAllAsync()
    {
        var data = await _api.GetAsync<List<ProvinceDTO>>("/api/Provinces");
        return data ?? new List<ProvinceDTO>();
    }
}
