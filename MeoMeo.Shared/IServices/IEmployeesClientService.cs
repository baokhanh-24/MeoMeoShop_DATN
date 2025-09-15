using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Employees;
using MeoMeo.Domain.Commons;
using Microsoft.AspNetCore.Http;

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
        Task<BaseResponse> UploadAvatarAsync(IFormFile file);
        Task<BaseResponse> ChangePasswordAsync(ChangePasswordDTO model);
        Task<CreateOrUpdateEmployeeResponseDTO> UpdateProfileAsync(CreateOrUpdateEmployeeDTO employee);
        Task<string> GetAvatarUrlAsync();
    }
}
