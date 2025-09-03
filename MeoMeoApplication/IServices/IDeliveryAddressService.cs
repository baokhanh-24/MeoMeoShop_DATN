using MeoMeo.Contract.DTOs;

namespace MeoMeo.Application.IServices
{
    public interface IDeliveryAddressService
    {
        Task<IEnumerable<DeliveryAddressDTO>> GetAllDeliveryAddressAsync();
        Task<DeliveryAddressDTO> GetDeliveryAddressByIdAsync(Guid id);
        Task<DeliveryAddressDTO> CreateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress);
        Task<DeliveryAddressDTO> UpdateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress);
        Task<bool> DeleteDeliveryAddressAsync(Guid id);
        Task<IEnumerable<DeliveryAddressDTO>> GetByCustomerIdAsync(Guid customerId);
    }
}
