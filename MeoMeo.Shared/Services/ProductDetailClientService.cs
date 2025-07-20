using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;

namespace MeoMeo.Shared.Services
{
    public class ProductDetailClientService : IProductDetailClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<ProductDetailClientService> _logger;

        public ProductDetailClientService(IApiCaller httpClient, ILogger<ProductDetailClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync(GetListProductDetailRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/ProductDetails/get-all-product-detail-async?{queryString}";
                var reponse = await _httpClient.GetAsync<PagingExtensions.PagedResult<ProductDetailDTO>>(url);
                return reponse ?? new PagingExtensions.PagedResult<ProductDetailDTO>();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách sản phẩm: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<ProductDetailDTO>();
            }
        }

        public async Task<ProductDetailDTO> GetProductDetailByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/ProductDetails/find-product-detail-by-id-async/{id}";
                var response = await _httpClient.GetAsync<ProductDetailDTO>(url);
                return response ?? new ProductDetailDTO();

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Có lỗi xảy ra khi lấy sản phẩm");
                return new ProductDetailDTO();
            }
        }
        public async Task<CreateOrUpdateProductDetailResponseDTO> CreateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail)
        {
            try
            {
                var url = "/api/ProductDetails/create-product-detail-async";
                var result = await _httpClient.PostAsync<CreateOrUpdateProductDetailDTO, CreateOrUpdateProductDetailResponseDTO>(url, productDetail);
                return result ?? new CreateOrUpdateProductDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo sản phẩm: {Message}", ex.Message);
                return new CreateOrUpdateProductDetailResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<CreateOrUpdateProductDetailResponseDTO> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail)
        {
            try
            {
                var url = $"/api/ProductDetails/update-product-detail-async/{productDetail.Id}";
                var result = await _httpClient.PutAsync<CreateOrUpdateProductDetailDTO, CreateOrUpdateProductDetailResponseDTO>(url, productDetail);
                return result ?? new CreateOrUpdateProductDetailResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật sản phẩm {Id}: {Message}", productDetail.Id, ex.Message);
                return new CreateOrUpdateProductDetailResponseDTO { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<bool> DeleteProductDetailAsync(Guid id)
        {
            try
            {
                var url = $"/api/ProductDetails/delete-product-detail-async/{id}";
                var success = await _httpClient.DeleteAsync(url);
                if (!success)
                {
                    _logger.LogWarning("Xóa sản phẩm thất bại với Id {Id}", id);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa sản phẩm {Id}: {Message}", id, ex.Message);
                return false;
            }
        }
    }
}
