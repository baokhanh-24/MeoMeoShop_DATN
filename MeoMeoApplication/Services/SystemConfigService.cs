using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Material;
using MeoMeo.Contract.DTOs.Size;
using MeoMeo.Contract.DTOs.SystemConfig;
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
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigRepository _systemConfigRepository;
        private readonly IMapper _mapper;
        public SystemConfigService(ISystemConfigRepository systemConfigRepository, IMapper mapper)
        {
            _systemConfigRepository = systemConfigRepository;
            _mapper = mapper;
        }

        public async Task<CreateOrUpdateSystemConfigResponseDTO> CreateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig)
        {
                var newSystemConfig = _mapper.Map<SystemConfig>(systemConfig);
                newSystemConfig.Id = Guid.NewGuid();
                await _systemConfigRepository.CreateSystemConfigAsync(newSystemConfig);
                var response = _mapper.Map<CreateOrUpdateSystemConfigResponseDTO>(newSystemConfig);
                response.Message = "System configuration created successfully.";
                response.ResponseStatus = BaseStatus.Success;
                return response;
        }

        public async Task<bool> DeleteSystemConfigAsync(Guid id)
        {
            var systemConfig = await _systemConfigRepository.GetSystemConfigByIdAsync(id);
            if (systemConfig == null)
            {
                return false;
            }
            return await _systemConfigRepository.DeleteSystemConfigAsync(id);
        }

        public async Task<PagingExtensions.PagedResult<SystemConfigDTO>> GetAllSystemConfigAsync(GetListSystemConfigRequestDTO request)
        {
            try
            {
                var query = _systemConfigRepository.Query();
                if (!string.IsNullOrEmpty(request.NameFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Name, $"%{request.NameFilter}%"));
                }
                if (!string.IsNullOrEmpty(request.ValueFilter))
                {
                    query = query.Where(c => EF.Functions.Like(c.Value, $"%{request.ValueFilter}%"));
                }
                if (request.TypeFilter != null)
                {
                    query = query.Where(x => x.Type == request.TypeFilter);
                }

                query = query.OrderByDescending(c => c.Name);
                var filteredCustomers = await _systemConfigRepository.GetPagedAsync(query, request.PageIndex, request.PageSize);
                var dtoItems = _mapper.Map<List<SystemConfigDTO>>(filteredCustomers.Items);

                return new PagingExtensions.PagedResult<SystemConfigDTO>
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

        public async Task<CreateOrUpdateSystemConfigResponseDTO> GetSystemConfigByIdAsync(Guid id)
        {
            var enity = await _systemConfigRepository.GetSystemConfigByIdAsync(id);
            if (enity == null)
            {
                return new CreateOrUpdateSystemConfigResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy cấu hình hệ thống này."
                };
            }
            return new CreateOrUpdateSystemConfigResponseDTO
            {
                Id = enity.Id,
                Name = enity.Name,
                Value = enity.Value,
                Type = enity.Type,
                ResponseStatus = BaseStatus.Success,
                Message = "Lấy thông tin cấu hình hệ thống thành công."
            };
        }

        public async Task<CreateOrUpdateSystemConfigResponseDTO> UpdateSystemConfigAsync(CreateOrUpdateSystemConfigDTO systemConfig)
        {
            var exisstingConfig = await _systemConfigRepository.GetSystemConfigByIdAsync(systemConfig.Id);
            if(exisstingConfig == null)
            {
                return new CreateOrUpdateSystemConfigResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không tìm thấy cấu hình hệ thống này."
                };
            }
            _mapper.Map(systemConfig, exisstingConfig);
            await _systemConfigRepository.UpdateSystemConfigAsync(exisstingConfig);
            var response = _mapper.Map<CreateOrUpdateSystemConfigResponseDTO>(exisstingConfig);
            response.Message = "Cập nhật cấu hình hệ thống thành công.";
            response.ResponseStatus = BaseStatus.Success;
            return response;
        }
    }
}
