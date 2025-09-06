using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Extended endpoints used by API controllers
        Task<List<BestSellerItemDTO>> GetWeeklyBestSellersAsync(int take = 10);
        Task<CategoryHoverResponseDTO> GetCategoryHoverPreviewAsync(Guid categoryId, int take = 6);
        Task<Dictionary<Guid, List<ProductResponseDTO>>> GetHeaderProductsAsync();

        // Recently viewed support
        Task<List<ProductResponseDTO>> GetProductsByIdsAsync(List<Guid> ids);

        // Featured products by rating
        Task<List<TopRatedProductDTO>> GetTopRatedProductsAsync(int take = 12);
    }
}
