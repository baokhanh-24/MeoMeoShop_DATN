using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class DeliveryAddressRepository : BaseRepository<DeliveryAddress>, IDeliveryAddressRepository
    {
        public DeliveryAddressRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<DeliveryAddress> CreateDeliveryAddressAsync(DeliveryAddress deliveryAddress)
        {
            var createDeliveryAddress = await AddAsync(deliveryAddress);
            return createDeliveryAddress;
        }

        public async Task<bool> DeleteDeliveryAddressAsync(DeliveryAddress deliveryAddress)
        {
            await DeleteAsync(deliveryAddress);
            return true;
        }

        public async Task<IEnumerable<DeliveryAddress>> GetAllDeliveryAddressAsync()
        {
            var deliveryAddress = await GetAllAsync();
            return deliveryAddress;
        }

        public async Task<DeliveryAddress> GetDeliveryAddressByIdAsync(Guid id)
        {
            var deliveryAddress = await GetByIdAsync(id);
            return deliveryAddress;
        }

        public async Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddress deliveryAddress)
        {
            var updateDeliveryAddress = await UpdateAsync(deliveryAddress);
            return updateDeliveryAddress;
        }
    }
}
