using MeoMeo.Contract.DTOs.VietQR;
using System.Text.Json;
using MeoMeo.Shared.IServices;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class VietQRClientService : IVietQRClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VietQRClientService> _logger;

        public VietQRClientService(HttpClient httpClient, ILogger<VietQRClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<BankDTO>> GetBanksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching banks from VietQR API");

                var response = await _httpClient.GetAsync("https://api.vietqr.io/v2/banks");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<VietQRBankResponseDTO>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Data != null)
                    {
                        _logger.LogInformation($"Successfully fetched {result.Data.Count} banks from VietQR API");
                        return result.Data;
                    }
                }

                _logger.LogWarning("Failed to fetch banks from VietQR API");
                return new List<BankDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching banks from VietQR API");
                return new List<BankDTO>();
            }
        }
    }
}