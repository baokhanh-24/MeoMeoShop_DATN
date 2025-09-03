using MeoMeo.Contract.DTOs;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services;

public class DeliveryAddressClientService : IDeliveryAddressClientService
{
    private readonly IApiCaller _api;

    public DeliveryAddressClientService(IApiCaller api)
    {
        _api = api;
    }

    public async Task<List<DeliveryAddressDTO>> GetMyAddressesAsync()
    {
        var data = await _api.GetAsync<List<DeliveryAddressDTO>>("/api/DeliveryAddresses/mine");
        return data ?? new List<DeliveryAddressDTO>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _api.DeleteAsync($"/api/DeliveryAddresses/delete-delivery-address/{id}");
    }

    public async Task<Guid?> CreateOrUpdateAsync(MeoMeo.Contract.DTOs.CreateOrUpdateDeliveryAddressDTO dto)
    {
        if (dto.Id.HasValue)
        {
            var resp = await _api.PutAsync<MeoMeo.Contract.DTOs.CreateOrUpdateDeliveryAddressDTO, DeliveryAddressDTO>($"/api/DeliveryAddresses/update-delivery-address/{dto.Id.Value}", dto);
            return resp?.Id;
        }
        else
        {
            var resp = await _api.PostAsync<MeoMeo.Contract.DTOs.CreateOrUpdateDeliveryAddressDTO, DeliveryAddressDTO>("/api/DeliveryAddresses/create-delivery-address", dto);
            return resp?.Id;
        }
    }
}


