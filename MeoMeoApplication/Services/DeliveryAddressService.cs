using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;

namespace MeoMeo.Application.Services
{
    public class DeliveryAddressService : IDeliveryAddressService
    {
        private readonly IDeliveryAddressRepository _deliveryAddressRepository;
        private readonly IMapper _mapper;

        public DeliveryAddressService(IDeliveryAddressRepository deliveryAddressRepository, IMapper mapper)
        {
            _deliveryAddressRepository = deliveryAddressRepository;
            _mapper = mapper;
        }

        public async Task<DeliveryAddress> CreateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress)
        {
            var mappedDeliveryAddress = _mapper.Map<DeliveryAddress>(deliveryAddress);
            mappedDeliveryAddress.Id = Guid.NewGuid();
            var result = await _deliveryAddressRepository.CreateDeliveryAddressAsync(mappedDeliveryAddress);
            return result;
        }

        public async Task<bool> DeleteDeliveryAddressAsync(Guid id)
        {
            var checkAddress = await _deliveryAddressRepository.GetByIdAsync(id);
            if (checkAddress.Id != id)
            {
                return false;
            }
            else
            {
                await _deliveryAddressRepository.DeleteAsync(checkAddress.Id);
                return true;
            }
        }

        public Task<IEnumerable<DeliveryAddress>> GetAllDeliveryAddressAsync()
        {
            return _deliveryAddressRepository.GetAllAsync();
        }

        public async Task<IEnumerable<DeliveryAddress>> GetByCustomerIdAsync(Guid customerId)
        {
            var query = _deliveryAddressRepository.Query().Where(a => a.CustomerId == customerId);
            return await Task.FromResult(query.AsEnumerable());
        }

        public async Task<DeliveryAddress> GetDeliveryAddressByIdAsync(Guid id)
        {
            var result = await _deliveryAddressRepository.GetByIdAsync(id);
            return result;
        }

        public async Task<DeliveryAddress> UpdateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress)
        {
            DeliveryAddress itemAddress = new DeliveryAddress();
            itemAddress = _mapper.Map<DeliveryAddress>(deliveryAddress);
            var result = await _deliveryAddressRepository.UpdateAsync(itemAddress);
            return result;
        }
    }
}
