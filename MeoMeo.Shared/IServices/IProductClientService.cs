using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IProductClientService
    {
        public Task<PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>> GetAllProductAsync(GetListProductRequestDTO request);
        public Task<CreateOrUpdateProductDTO> GetProductDetailAsync(Guid id);
        public Task<ProductResponseDTO> GetProductWithDetailsAsync(Guid id);
        public Task<BaseResponse> CreateProductAsync(CreateOrUpdateProductDTO product);
        public Task<BaseResponse> UpdateProductAsync(CreateOrUpdateProductDTO product);
        public Task<BaseResponse> UpdateProductVariantStatusAsync(UpdateProductStatusDTO input);
        public Task<BaseResponse> DeleteProductAsync(Guid id);
        // Home page helpers
        public Task<List<BestSellerItemDTO>> GetWeeklyBestSellersAsync(int take = 10);
        public Task<List<ProductResponseDTO>> GetByIdsAsync(List<Guid> ids);
        public Task<Dictionary<Guid, List<ProductResponseDTO>>> GetHeaderProductsAsync();
        // Search methods
        public Task<PagingExtensions.PagedResult<ProductSearchResponseDTO>> SearchProductsAsync(ProductSearchRequestDTO request);
        public Task<ProductSearchResponseDTO?> GetProductBySkuAsync(string sku);
    }
}
