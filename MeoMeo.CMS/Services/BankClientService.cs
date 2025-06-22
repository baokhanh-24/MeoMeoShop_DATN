using MeoMeo.CMS.IServices;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Utilities;

namespace MeoMeo.CMS.Services
{
    public class BankClientService : IBankClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<BankClientService> _logger;

        public BankClientService(IApiCaller httpClient, ILogger<BankClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<BankDTO>> GetAllBankAsync(GetListBankRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Bank/get-paging-bank-async?{queryString}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<BankDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<BankDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách Bank: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<BankDTO>();
            }
        }
    }
}
