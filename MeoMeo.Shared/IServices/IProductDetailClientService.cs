using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.IServices
{
    public interface IProductDetailClientService
    {
        public Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync(GetListProductDetailRequestDTO request);
        public Task<CreateOrUpdateProductDetailDTO> GetProductDetailByIdAsync(Guid id);
        public Task<CreateOrUpdateProductDetailResponseDTO> CreateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail);
        public Task<CreateOrUpdateProductDetailResponseDTO> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail);
        public Task<bool> DeleteProductDetailAsync(Guid id);
    }
}
