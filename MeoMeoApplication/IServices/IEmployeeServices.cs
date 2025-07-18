using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
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
        Task<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>> GetAllEmployeeAsync(GetlistEmployeesRequestDTO requestDTO);
        Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeeByIdAsyncccc(Guid id);
        Task<CreateOrUpdateEmployeeResponseDTO> CreateEmployeeAsync(CreateOrUpdateEmployeeDTO employee);
        Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeeAsync(CreateOrUpdateEmployeeDTO employee);
        Task<bool> DeleteEmployeeAsync(Guid id);
    }
}
