using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IEmployeeServices
    {
        Task<List<Employee>> GetAllEmployeeAsync();
        Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeeByIdAsync(Guid id);
        Task<Employee> CreateEmployeeAsync(CreateOrUpdateEmployeeDTO employee);
        Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeeAsync(CreateOrUpdateEmployeeDTO employee);
        Task<bool> DeleteEmployeeAsync(Guid id);
    }
}
