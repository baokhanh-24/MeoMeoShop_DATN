using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class ProductClientService : IProductClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<ProductClientService> _logger;

        public ProductClientService(IApiCaller httpClient, ILogger<ProductClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<ProductDTO>> GetAllProductAsync(GetListProductRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Product/get-all-product-async?{queryString}";
                var reponse = await _httpClient.GetAsync<PagingExtensions.PagedResult<ProductDTO>>(url);
                return reponse ?? new PagingExtensions.PagedResult<ProductDTO>();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Có lỗi khi xảy ra khi lấy danh sách sản phẩm: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<ProductDTO>();
            }
        }

        public async Task<ApiResponse<DetailProductViewDto>> GetProductDetailAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<bool>> UpdateProductStatusAsync(Guid id, int status)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<List<ProductHistoryDto>>> GetProductHistoryAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
