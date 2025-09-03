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

        public async Task<DeliveryAddressDTO> CreateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress)
        {
            try
            {
                var mappedDeliveryAddress = _mapper.Map<DeliveryAddress>(deliveryAddress);
                mappedDeliveryAddress.Id = Guid.NewGuid();

                // Set default values for Province, District, Commune if not provided
                if (!deliveryAddress.ProvinceId.HasValue || deliveryAddress.ProvinceId == null)
                    mappedDeliveryAddress.ProvinceId = -1;
                if (!deliveryAddress.DistrictId.HasValue || deliveryAddress.DistrictId == null)
                    mappedDeliveryAddress.DistrictId =  -1;
                if (!deliveryAddress.CommuneId.HasValue || deliveryAddress.CommuneId == null)
                    mappedDeliveryAddress.CommuneId =  -1;

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

        public async Task<IEnumerable<DeliveryAddressDTO>> GetAllDeliveryAddressAsync()
        {
            var addresses = await _deliveryAddressRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DeliveryAddressDTO>>(addresses);
        }

        public async Task<IEnumerable<DeliveryAddressDTO>> GetByCustomerIdAsync(Guid customerId)
        {
            var query = _deliveryAddressRepository.Query().Where(a => a.CustomerId == customerId);
            var addresses = await Task.FromResult(query.AsEnumerable());
            return _mapper.Map<IEnumerable<DeliveryAddressDTO>>(addresses);
        }

        public async Task<DeliveryAddressDTO> GetDeliveryAddressByIdAsync(Guid id)
        {
            var result = await _deliveryAddressRepository.GetByIdAsync(id);
            return _mapper.Map<DeliveryAddressDTO>(result);
        }

        public async Task<DeliveryAddressDTO> UpdateDeliveryAddressAsync(CreateOrUpdateDeliveryAddressDTO deliveryAddress)
        {
            if (!deliveryAddress.Id.HasValue)
                throw new ArgumentException("Id is required for update");
                
            var existingAddress = await _deliveryAddressRepository.GetByIdAsync(deliveryAddress.Id.Value);
            if (existingAddress == null)
                throw new ArgumentException("Delivery address not found");
                
            // Update only the fields that are provided
            existingAddress.Name = deliveryAddress.Name;
            existingAddress.PhoneNumber = deliveryAddress.PhoneNumber;
            existingAddress.Address = deliveryAddress.Address;
            
            if (deliveryAddress.ProvinceId.HasValue)
                existingAddress.ProvinceId = deliveryAddress.ProvinceId.Value;
            if (deliveryAddress.DistrictId.HasValue)
                existingAddress.DistrictId = deliveryAddress.DistrictId.Value;
            if (deliveryAddress.CommuneId.HasValue)
                existingAddress.CommuneId = deliveryAddress.CommuneId.Value;
                
            var result = await _deliveryAddressRepository.UpdateAsync(existingAddress);
            return _mapper.Map<DeliveryAddressDTO>(result);
        }
    }
}
