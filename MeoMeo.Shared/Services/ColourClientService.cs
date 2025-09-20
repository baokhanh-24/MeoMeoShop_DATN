using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;
using static MeoMeo.Domain.Commons.PagingExtensions;

namespace MeoMeo.Shared.Services
{
    public class ColourClientService : IColourClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<ColourClientService> _logger;

        public ColourClientService(IApiCaller httpClient, ILogger<ColourClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<Colour>> GetAllColoursAsync()
        {
            try
            {
                var url = "/api/Colour";
                var response = await _httpClient.GetAsync<IEnumerable<Colour>>(url);
                return response ?? new List<Colour>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Colour: {Message}", ex.Message);
                return new List<Colour>();
            }
        }

        public async Task<PagedResult<ColourDTO>> GetAllColoursPagedAsync(GetListColourRequest request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Colour/get-all-colours-paged?{queryString}";
                var response = await _httpClient.GetAsync<PagedResult<ColourDTO>>(url);
                return response ?? new PagedResult<ColourDTO>
                {
                    TotalRecords = 0,
                    PageIndex = 1,
                    PageSize = 10,
                    Items = new List<ColourDTO>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách Colour phân trang: {Message}", ex.Message);
                return new PagedResult<ColourDTO>
                {
                    TotalRecords = 0,
                    PageIndex = 1,
                    PageSize = 10,
                    Items = new List<ColourDTO>()
                };
            }
        }

        public async Task<ColourResponseDTO> GetColourByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/Colour/{id}";
                var response = await _httpClient.GetAsync<ColourResponseDTO>(url);
                return response ?? new ColourResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy Colour theo Id {Id}: {Message}", id, ex.Message);
                return new ColourResponseDTO();
            }
        }

        public async Task<ColourResponseDTO> CreateColourAsync(ColourDTO colourDTO)
        {
            try
            {
                var url = "/api/Colour";
                var result = await _httpClient.PostAsync<ColourDTO, ColourResponseDTO>(url, colourDTO);
                return result ?? new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo Colour: {Message}", ex.Message);
                return new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo Colour"
                };
            }
        }

        public async Task<ColourResponseDTO> UpdateColourAsync(ColourDTO colourDTO)
        {
            try
            {
                var url = $"/api/Colour/{colourDTO.Id}";
                var result = await _httpClient.PutAsync<ColourDTO, ColourResponseDTO>(url, colourDTO);
                return result ?? new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật Colour: {Message}", ex.Message);
                return new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật Colour"
                };
            }
        }

        public async Task<ColourResponseDTO> DeleteColourAsync(Guid id)
        {
            try
            {
                var url = $"/api/Colour/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Xóa Colour thất bại với Id {Id}", id);
                }
                return new ColourResponseDTO
                {
                    ResponseStatus = result ? BaseStatus.Success : BaseStatus.Error,
                    Message = result ? "Xóa thành công" : "Xóa thất bại"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xoá Colour {Id}: {Message}", id, ex.Message);
                return new ColourResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi xóa Colour"
                };
            }
        }
    }
}