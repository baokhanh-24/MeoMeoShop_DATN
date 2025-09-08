using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Permission;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MeoMeo.Application.IServices
{
    public interface IPermissionGroupService
    {
        Task<List<PermissionGroupDTO>> GetAllPermissionGroupsAsync();
        Task<List<PermissionGroupDTO>> GetPermissionGroupsTreeAsync();
        Task<PermissionGroupDTO> GetPermissionGroupByIdAsync(Guid id);
        Task<BaseResponse> CreatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto);
        Task<BaseResponse> UpdatePermissionGroupAsync(CreateOrUpdatePermissionGroupDTO dto);
        Task<BaseResponse> DeletePermissionGroupAsync(Guid id);
    }
}
