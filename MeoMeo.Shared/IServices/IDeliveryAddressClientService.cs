using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.IServices;

public interface IDeliveryAddressClientService
{
    Task<List<DeliveryAddressItem>> GetMyAddressesAsync();
    Task<bool> DeleteAsync(Guid id);
    Task<Guid?> CreateOrUpdateAsync(CreateOrUpdateDeliveryAddressDTO dto);
}

public class DeliveryAddressItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}


