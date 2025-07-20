using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IEmployeesClientService
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdateEmployeeDTO>> GetAllEmployeesAsync(GetlistEmployeesRequestDTO request);
        Task<CreateOrUpdateEmployeeResponseDTO> GetEmployeesByIdAsync(Guid id);
        Task<CreateOrUpdateEmployeeResponseDTO> CreateEmployeesAsync(CreateOrUpdateEmployeeDTO employee);
        Task<CreateOrUpdateEmployeeResponseDTO> UpdateEmployeesAsync(CreateOrUpdateEmployeeDTO employee);
        Task<CreateOrUpdateEmployeeResponseDTO> ChangePasswordAsync(ChangePasswordRequestDTO dto);
        Task<bool> DeleteEmployeesAsync(Guid id);
    }
}
