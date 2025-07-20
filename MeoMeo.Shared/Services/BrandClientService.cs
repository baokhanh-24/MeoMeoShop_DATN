
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class BrandClientService : IBrandClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<BrandClientService> _logger;

        public BrandClientService(IApiCaller httpClient, ILogger<BrandClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            try
            {
                var url = $"/api/Brands/delete-brand-async/{id}";
                var reuslt = await _httpClient.DeleteAsync(url);
                if (!reuslt)
                {
                    _logger.LogWarning("Xóa brand thất bại với Id {Id}", id);
                }
                return reuslt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá Brand {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<PagingExtensions.PagedResult<BrandDTO>> GetAllBrandAsync(GetListBrandRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Brands/get-all-brand-async?{queryString}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<BrandDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<BrandDTO>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Brand: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<BrandDTO>();
            }
        }

        public async Task<BrandDTO> GetBrandByIdAsync(Guid id)
        {

            try
            {
                var url = $"/api/Brands/find-brand-by-id-async/{id}";
                var response = await _httpClient.GetAsync<BrandDTO>(url);
                return response ?? new BrandDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy Brand theo Id {Id}: {Message}", id, ex.Message);
                return new BrandDTO();
            }
        }

        public async Task<CreateOrUpdateBrandResponseDTO> CreateBrandAsync(CreateOrUpdateBrandDTO brandDto)
        {
            try
            {
                var url = "/api/Brands/create-brand-async";

                var result = await _httpClient.PostAsync<CreateOrUpdateBrandDTO, CreateOrUpdateBrandResponseDTO>(url, brandDto);

                return result ?? new CreateOrUpdateBrandResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về "
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo Brand: {Message}", ex.Message);
                return new CreateOrUpdateBrandResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message }; // fallback trả về rỗng nếu có lỗi
            }
        }


        public async Task<CreateOrUpdateBrandResponseDTO> UpdateBrandAsync(CreateOrUpdateBrandDTO brandDto)
        {
            try
            {
                var url = $"/api/Brands/update-brand-async/{brandDto.Id}";

                var result = await _httpClient.PutAsync<CreateOrUpdateBrandDTO, CreateOrUpdateBrandResponseDTO>(url, brandDto);

                return result ?? new CreateOrUpdateBrandResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về từ API."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Brand {Id}: {Message}", brandDto.Id, ex.Message);

                return new CreateOrUpdateBrandResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

    }
}
