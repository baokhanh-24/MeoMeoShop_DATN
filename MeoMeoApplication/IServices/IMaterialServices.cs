using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IMaterialServices
    {
        Task<PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>> GetAllMaterialsAsync(GetListMaterialRequest request);
        Task<CreateOrUpdateMaterialResponse> GetMaterialsByIdAsync(Guid id);
        Task<CreateOrUpdateMaterialResponse> CreateMaterialsAsync(CreateOrUpdateMaterialDTO material);
        Task<CreateOrUpdateMaterialResponse> UpdateMaterialsAsync(CreateOrUpdateMaterialDTO material);
        Task<CreateOrUpdateMaterialResponse> DeleteMaterialsAsync(Guid id);
    }
}
