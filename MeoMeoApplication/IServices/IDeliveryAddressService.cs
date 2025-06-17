using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IDeliveryAddressService
    {
        Task<IEnumerable<DeliveryAddress>> GetAllDeliveryAddressAsync();
        Task<DeliveryAddress> GetDeliveryAddressByIdAsync(Guid id);
        Task<DeliveryAddress> CreateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress);
        Task<DeliveryAddress> UpdateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress);
        Task<bool> DeleteDeliveryAddressAsync(Guid id);
    }
}
