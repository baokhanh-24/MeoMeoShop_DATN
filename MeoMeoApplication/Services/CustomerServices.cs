using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerServices(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Customers> CreateCustomersAsync(CreateOrUpdateCustomerDTO customer)
        {
            var mappedcustomer = _mapper.Map<Customers>(customer);
            mappedcustomer.Id = Guid.NewGuid();
            return await _repository.CreateCustomersAsync(mappedcustomer);
        }

        public async Task<bool> DeleteCustomersAsync(Guid id)
        {
            var customerToDelete = await _repository.GetCustomersByIdAsync(id);

            if (customerToDelete == null)
            {
                return false;
            }

            await _repository.DeleteCustomersAsync(customerToDelete.Id);
            return true;
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await _repository.GetAllCustomersAsync();
        }

        public async Task<CreateOrUpdateCustomerResponseDTO> GetCustomersByIdAsync(Guid id)
        {
            CreateOrUpdateCustomerResponseDTO responseDTO = new CreateOrUpdateCustomerResponseDTO();

            var check = await _repository.GetCustomersByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy customer";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdateCustomerResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdateCustomerResponseDTO> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer)
        {
            var itemCustomers = await _repository.GetCustomersByIdAsync(Guid.Parse(customer.Id.ToString()));
            if (itemCustomers == null)
            {
                return new CreateOrUpdateCustomerResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy customer" };
            }
            _mapper.Map(customer, itemCustomers);

            await _repository.UpdateCustomersAsync(itemCustomers);
            return new CreateOrUpdateCustomerResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
