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
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeServices(IEmployeeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Employee> CreateEmployeeAsync(CreateOrUpdateEmployeeDTO employee)
        {
            var mappedEmployee = _mapper.Map<Employee>(employee);
            mappedEmployee.Id = Guid.NewGuid();
            return await _repository.CreateEmployeeAsync(mappedEmployee);
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            var employeeToDelete = await _repository.GetEmployeeByIdAsync(id);

            if (employeeToDelete == null)
            {
                return false;
            }

            await _repository.DeleteEmployeeAsync(employeeToDelete.Id);
            return true;
        }

        public async Task<List<Employee>> GetAllEmployeeAsync()
        {
            return await _repository.GetAllEmployeeAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(Guid id)
        {
            return await _repository.GetEmployeeByIdAsync(id);
        }

        public async Task<Employee> UpdateEmployeeAsync(CreateOrUpdateEmployeeDTO employee)
        {
            Employee employeeCheck = new Employee();

            employeeCheck = _mapper.Map<Employee>(employee);

            var result = await _repository.UpdateEmployeeAsync(employeeCheck);

            return result;
        }
    }
}
