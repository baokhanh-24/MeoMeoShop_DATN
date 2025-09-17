using MeoMeo.Contract.DTOs;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MeoMeo.Shared.Services
{
    public class InventoryStatisticsClientService : IInventoryStatisticsClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<InventoryStatisticsClientService> _logger;

        public InventoryStatisticsClientService(IApiCaller httpClient, ILogger<InventoryStatisticsClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GetInventoryStatisticsResponseDTO> GetInventoryStatisticsAsync(GetInventoryStatisticsRequestDTO request)
        {
            try
            {
                var url = "/api/InventoryStatistics/get-statistics";
                var response = await _httpClient.PostAsync<GetInventoryStatisticsRequestDTO, GetInventoryStatisticsResponseDTO>(url, request);
                return response ?? new GetInventoryStatisticsResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tồn kho: {Message}", ex.Message);
                return new GetInventoryStatisticsResponseDTO();
            }
        }

        public async Task<GetInventoryHistoryResponseDTO> GetInventoryHistoryAsync(GetInventoryHistoryRequestDTO request)
        {
            try
            {
                var url = "/api/InventoryStatistics/get-history";
                var response = await _httpClient.PostAsync<GetInventoryHistoryRequestDTO, GetInventoryHistoryResponseDTO>(url, request);
                return response ?? new GetInventoryHistoryResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử tồn kho: {Message}", ex.Message);
                return new GetInventoryHistoryResponseDTO();
            }
        }
    }
}
