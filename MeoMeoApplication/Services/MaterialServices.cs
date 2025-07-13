using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class MaterialServices : IMaterialServices
    {
        private readonly IMaterialRepository _repository;
        private readonly IMapper _mapper;

        public MaterialServices(IMaterialRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CreateOrUpdateMaterialResponse> CreateMaterialsAsync(CreateOrUpdateMaterialDTO material)
        {
            var newMaterial = _mapper.Map<Material>(material);
            newMaterial.Id = Guid.NewGuid(); 
            await _repository.CreateMaterialAsync(newMaterial);
            var response = _mapper.Map<CreateOrUpdateMaterialResponse>(newMaterial);
            response.Message = "Material created successfully.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        public async Task<CreateOrUpdateMaterialResponse> DeleteMaterialsAsync(Guid id)
        {
            var material = await _repository.GetMaterialByIdAsync(id);
            if(material == null)
            {
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy chất liệu này."
                };
            }
            await _repository.DeleteMaterialAsync(id);
            return new CreateOrUpdateMaterialResponse
            {
                ResponseStatus = BaseStatus.Success,
                Message = "Material deleted successfully."
            };
        }

        public async Task<PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>> GetAllMaterialsAsync(GetListMaterialRequest request)
        {
           try
            {
                var query = _repository.Query();
                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{request.NameFilter}%"));
                }
                if (request.DurabilityFilter != null)
                {
                    query = query.Where(x => x.Durability == request.DurabilityFilter);
                }
                if (request.WaterProofFilter != null)
                {
                    query = query.Where(x => x.WaterProof == request.WaterProofFilter);
                }
                if (request.WeightFilter != null)
                {
                    query = query.Where(x => x.Weight == request.WeightFilter);
                }
                if (!string.IsNullOrEmpty(request.DescriptionFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Description, $"%{request.DescriptionFilter}%"));
                }
                if (request.StatusFilter != null)
                {
                    query = query.Where(x => x.Status == (int)request.StatusFilter);
                }

                query = query.OrderByDescending(c => c.Name);
                var filteredCustomers = await _repository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<CreateOrUpdateMaterialDTO>>(filteredCustomers.Items);

                return new PagingExtensions.PagedResult<CreateOrUpdateMaterialDTO>
                {
                    TotalRecords = filteredCustomers.TotalRecords,
                    PageIndex = filteredCustomers.PageIndex,
                    PageSize = filteredCustomers.PageSize,
                    Items = dtoItems
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllSizeAsync: {ex.Message}, StackTrace: {ex.StackTrace}");

                throw;
            }   
        }

        public async Task<CreateOrUpdateMaterialResponse> GetMaterialsByIdAsync(Guid id)
        {
            var entity = await  _repository.GetMaterialByIdAsync(id);
            if(entity == null)
            {
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy chất liệu này."
                };
            }
            return new CreateOrUpdateMaterialResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Durability = entity.Durability,
                WaterProof = entity.WaterProof,
                Weight = entity.Weight,
                Description = entity.Description,
                Status = entity.Status,
                ResponseStatus = BaseStatus.Success,
                Message = "Material retrieved successfully."
            };
        }

        public async Task<CreateOrUpdateMaterialResponse> UpdateMaterialsAsync(CreateOrUpdateMaterialDTO material)
        {
            var existingMaterial = await _repository.GetMaterialByIdAsync(material.Id);
            if (existingMaterial == null) 
            {
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy chất liệu này."
                };
            }

            _mapper.Map(material, existingMaterial);
            await _repository.UpdateMaterialAsync(existingMaterial);

            var response = _mapper.Map<CreateOrUpdateMaterialResponse>(existingMaterial);
            response.Message = "Material updated successfully.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }

        public async Task<CreateOrUpdateMaterialResponse> UpdateMaterialStatusAsync(UpdateStatusRequestDTO dto)
        {
            var material = await _repository.GetMaterialByIdAsync(dto.Id);

            if (material == null)
            {
                return new CreateOrUpdateMaterialResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy chất liệu."
                };
            }

            material.Status = dto.Status;

            await _repository.UpdateMaterialAsync(material);

            var response = _mapper.Map<CreateOrUpdateMaterialResponse>(material);
            response.ResponseStatus = BaseStatus.Success;
            response.Message = "Cập nhật trạng thái chất liệu thành công.";

            return response;
        }
    }
}
