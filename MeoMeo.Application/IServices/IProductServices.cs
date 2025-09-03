using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Entities;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Application.IServices
{
    public interface IProductServices
    {
        Task<CreateOrUpdateProductDTO> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductResponseDTO>> GetAllAsync();
        Task<BaseResponse> CreateProductAsync(CreateOrUpdateProductDTO product, List<FileUploadResult> uploadedFiles);
        Task<BaseResponse> UpdateAsync(CreateOrUpdateProductDTO model, List<FileUploadResult> uploadedFiles);
        Task<ProductResponseDTO> DeleteAsync(Guid id);

        Task<PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>> GetPagedProductsAsync(GetListProductRequestDTO request);
        Task<ProductResponseDTO> GetProductWithDetailsAsync(Guid id);
        Task<BaseResponse> UpdateVariantStatusAsync(UpdateProductStatusDTO input);
        Task<List<Image>> GetOldImagesAsync(Guid productId);

        // New method for home page
        Task<HomePageResponseDTO> GetHomePageProductsAsync(int discountedProductsLimit = 10, int bestSellingProductsLimit = 10);

        // New: Weekly best sellers and category previews
        Task<List<BestSellerItemDTO>> GetWeeklyBestSellersAsync(int take = 10);
        Task<CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(Guid categoryId, int take = 6);

        // Recently viewed support
        Task<List<ProductResponseDTO>> GetProductsByIdsAsync(List<Guid> ids); ter
    }
}
