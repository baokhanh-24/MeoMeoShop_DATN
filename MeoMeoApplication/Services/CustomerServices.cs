using AutoMapper;
using MeoMeo.Application.IServices;
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

        public async Task<Customers> GetCustomersByIdAsync(Guid id)
        {
            return await _repository.GetCustomersByIdAsync(id);
        }

        public async Task<Customers> UpdateCustomersAsync(CreateOrUpdateCustomerDTO customer)
        {
            Customers customerCheck = new Customers();

            customerCheck = _mapper.Map<Customers>(customer);

            var result = await _repository.UpdateCustomersAsync(customerCheck);

            return result;
        }
    }
}
