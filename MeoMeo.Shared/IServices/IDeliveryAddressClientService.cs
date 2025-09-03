using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.IServices;

public interface IDeliveryAddressClientService
{
    Task<List<DeliveryAddressDTO>> GetMyAddressesAsync();
    Task<bool> DeleteAsync(Guid id);
    Task<Guid?> CreateOrUpdateAsync(CreateOrUpdateDeliveryAddressDTO dto);
}
