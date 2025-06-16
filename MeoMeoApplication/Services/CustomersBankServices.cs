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
    public class CustomersBankServices : ICustomersBankServices
    {
        private readonly ICustomersBankRepository _repository;
        private readonly IMapper _mapper;

        public CustomersBankServices(ICustomersBankRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CustomersBank> CreateCustomersBankAsync(CreateOrUpdateCustomersBankDTO customersBank)
        {
            var mappedCustomersBank = _mapper.Map<CustomersBank>(customersBank);
            mappedCustomersBank.Id = Guid.NewGuid();
            return await _repository.CreateCustomersBankAsync(mappedCustomersBank);
        }

        public async Task<bool> DeleteCustomersBankAsync(Guid id)
        {
            var customersBankToDelete = await _repository.GetCustomersBankByIdAsync(id);

            if (customersBankToDelete == null)
            {
                return false;
            }

            await _repository.DeleteCustomersBankAsync(customersBankToDelete.Id);
            return true;
        }

        public async Task<List<CustomersBank>> GetAllCustomersBankAsync()
        {
            return await _repository.GetAllCustomersBankAsync();
        }

        public async Task<CustomersBank> GetCustomersBankByIdAsync(Guid id)
        {
            return await _repository.GetCustomersBankByIdAsync(id);
        }

        public async Task<CustomersBank> UpdateCustomersBankAsync(CreateOrUpdateCustomersBankDTO customersBank)
        {
            CustomersBank customersBankCheck = new CustomersBank();

            customersBankCheck = _mapper.Map<CustomersBank>(customersBank);

            var result = await _repository.UpdateCustomersBankAsync(customersBankCheck);

            return result;
        }
    }
}
