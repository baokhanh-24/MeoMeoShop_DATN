using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IProductClientService
    {
        public Task<PagingExtensions.PagedResult<ProductDTO>> GetAllProductAsync(GetListProductRequestDTO request);
        public Task<ApiResponse<DetailProductViewDto>> GetProductDetailAsync(Guid id);
        public Task<ApiResponse<bool>> UpdateProductStatusAsync(Guid id, int status);
        public Task<ApiResponse<bool>> DeleteProductAsync(Guid id);
        public Task<ApiResponse<List<ProductHistoryDto>>> GetProductHistoryAsync(Guid id);
    }
}
