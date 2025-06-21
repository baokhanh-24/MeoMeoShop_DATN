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

        public async Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeeByIdAsync(Guid id)
        {
            CreateOrUpdateEmployeeResponseDTO responseDTO = new CreateOrUpdateEmployeeResponseDTO();

            var check = await _repository.GetEmployeeByIdAsync(id);
            if (check == null)
            {
                responseDTO.ResponseStatus = BaseStatus.Error;
                responseDTO.Message = "Không tìm thấy employee";
                return responseDTO;
            }

            responseDTO = _mapper.Map<CreateOrUpdateEmployeeResponseDTO>(check);
            responseDTO.ResponseStatus = BaseStatus.Success;
            responseDTO.Message = "";
            return responseDTO;
        }

        public async Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeeAsync(CreateOrUpdateEmployeeDTO employee)
        {
            var itemEmployee = await _repository.GetEmployeeByIdAsync(Guid.Parse(employee.Id.ToString()));
            if (itemEmployee == null)
            {
                return new CreateOrUpdateEmployeeResponseDTO { ResponseStatus = BaseStatus.Error, Message = "Không tìm thấy employee" };
            }
            _mapper.Map(employee, itemEmployee);

            await _repository.UpdateEmployeeAsync(itemEmployee);
            return new CreateOrUpdateEmployeeResponseDTO { ResponseStatus = BaseStatus.Success, Message = "Cập nhật thành công" };
        }
    }
}
