using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;

namespace MeoMeo.Application.IServices
{
    public interface IProductDetailServices
    {

        public Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync(GetListProductDetailRequestDTO request);
        public Task<ProductDetailDTO> GetProductDetailByIdAsync(Guid id);
        public Task<BaseResponse> CreateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail, List<FileUploadResult> lstFileMedia );
        public Task<BaseResponse> UpdateProductDetailAsync(CreateOrUpdateProductDetailDTO productDetail, List<FileUploadResult> lstFileMedia);
        public Task<bool> DeleteProductDetailAsync(Guid id);
        public Task<List<Image>> GetOldImagesAsync(Guid productDetailId);

    }
}
