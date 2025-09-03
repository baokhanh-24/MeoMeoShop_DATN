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
            try
            {
                var mappedDeliveryAddress = _mapper.Map<DeliveryAddress>(deliveryAddress);
                mappedDeliveryAddress.Id = Guid.NewGuid();

                // Set default values for Province, District, Commune if not provided
                if (!deliveryAddress.ProvinceId.HasValue || deliveryAddress.ProvinceId == null)
                    mappedDeliveryAddress.ProvinceId = -1;
                if (!deliveryAddress.DistrictId.HasValue || deliveryAddress.DistrictId == null)
                    mappedDeliveryAddress.DistrictId = -1;
                if (!deliveryAddress.CommuneId.HasValue || deliveryAddress.CommuneId == null)
                    mappedDeliveryAddress.CommuneId = -1;

                var result = await _deliveryAddressRepository.CreateDeliveryAddressAsync(mappedDeliveryAddress);
                return _mapper.Map<DeliveryAddressDTO>(result);
            }
            catch (Exception e)
            {
                return new DeliveryAddressDTO();
            }
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
