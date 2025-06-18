using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Domain.IRepositories
{
    public interface IDeliveryAddressRepository : IBaseRepository<DeliveryAddress>
    {
        Task<IEnumerable<DeliveryAddress>> GetAllDeliveryAddressAsync();
        public Task<DeliveryAddress> GetDeliveryAddressByIdAsync(Guid id);
        public Task<DeliveryAddress> CreateDeliveryAddressAsync(DeliveryAddress deliveryAddress);
        public Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddress deliveryAddress);
        public Task<bool> DeleteDeliveryAddressAsync(DeliveryAddress deliveryAddress);
    }
}
