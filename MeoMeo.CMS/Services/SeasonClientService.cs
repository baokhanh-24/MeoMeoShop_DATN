
using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.CMS.Services
{
    public class SeasonClientService : ISeasonClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<SeasonClientService> _logger;

        public SeasonClientService(IApiCaller httpClient, ILogger<SeasonClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<SeasonDTO>> GetAllSeasonsAsync(GetListSeasonRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Seasons/get-all-season-async?{queryString}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<SeasonDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<SeasonDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Season: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<SeasonDTO>();
            }
        }

        public async Task<SeasonDTO> GetSeasonByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/Seasons/find-season-by-id-async/{id}";
                var response = await _httpClient.GetAsync<SeasonDTO>(url);
                return response ?? new SeasonDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy Season theo Id {Id}: {Message}", id, ex.Message);
                return new SeasonDTO();
            }
        }

        public async Task<CreateOrUpdateSeasonResponseDTO> CreateSeasonAsync(CreateOrUpdateSeasonDTO dto)
        {
            try
            {
                var url = "/api/Seasons/create-season-async";
                var response = await _httpClient.PostAsync<CreateOrUpdateSeasonDTO, CreateOrUpdateSeasonResponseDTO>(url, dto);
                return response ?? new CreateOrUpdateSeasonResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo Season: {Message}", ex.Message);
                return new CreateOrUpdateSeasonResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<CreateOrUpdateSeasonResponseDTO> UpdateSeasonAsync(CreateOrUpdateSeasonDTO dto)
        {
            try
            {
                var url = $"/api/Seasons/update-season-async/{dto.Id}";
                var response = await _httpClient.PutAsync<CreateOrUpdateSeasonDTO, CreateOrUpdateSeasonResponseDTO>(url, dto);
                return response ?? new CreateOrUpdateSeasonResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật Season {Id}: {Message}", dto.Id, ex.Message);
                return new CreateOrUpdateSeasonResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> DeleteSeasonAsync(Guid id)
        {
            try
            {
                var url = $"/api/Seasons/delete-season-async/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Xóa Season thất bại với Id {Id}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá Season {Id}: {Message}", id, ex.Message);
                return false;
            }
        }
    }
}