using System.Globalization;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Product;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.IO;

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

        public async Task<PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>>
            GetAllProductAsync(GetListProductRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/Products/get-paged-products-async?{queryString}";
                var response = await _httpClient
                    .GetAsync<PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi xảy ra khi lấy danh sách sản phẩm: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<ProductResponseDTO, GetListProductResponseDTO>();
            }
        }

        public async Task<CreateOrUpdateProductDTO> GetProductDetailAsync(Guid id)
        {
            try
            {
                var url = $"/api/Products/get-product-by-id-async/{id}";
                var response = await _httpClient.GetAsync<CreateOrUpdateProductDTO>(url);
                return response ?? new CreateOrUpdateProductDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy sản phẩm");
                return new CreateOrUpdateProductDTO();
            }
        }

        public async Task<ProductResponseDTO> GetProductWithDetailsAsync(Guid id)
        {
            try
            {
                var url = $"/api/Products/get-product-with-details/{id}";
                var response = await _httpClient.GetAsync<ProductResponseDTO>(url);
                return response ?? new ProductResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi lấy chi tiết sản phẩm: {Message}", ex.Message);
                return new ProductResponseDTO();
            }
        }

        public async Task<BaseResponse> CreateProductAsync(CreateOrUpdateProductDTO product)
        {
            try
            {
                var url = "/api/Products/create-product-async";
                var formData = ConvertToFormData(product);
                var result = await _httpClient.PostFormAsync<BaseResponse>(url, formData);
                return result ?? new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo sản phẩm: {Message}", ex.Message);
                return new BaseResponse
                { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdateProductAsync(CreateOrUpdateProductDTO product)
        {
            try
            {
                var url = $"/api/Products/update-product-async";
                var formData = ConvertToFormData(product);
                var result = await _httpClient.PutFormAsync<BaseResponse>(url, formData);
                return result ?? new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật sản phẩm {Id}: {Message}", product.Id,
                    ex.Message);
                return new BaseResponse
                { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> UpdateProductVariantStatusAsync(UpdateProductStatusDTO input)
        {
            try
            {
                var url = $"/api/Products/update-variant-status-async";
                var result = await _httpClient.PutAsync<UpdateProductStatusDTO, BaseResponse>(url, input);
                return result ?? new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật trạng thái sản phẩm {Id}: {Message}", input.Id,
                    ex.Message);
                return new BaseResponse
                { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> DeleteProductAsync(Guid id)
        {
            try
            {
                var url = $"/api/Products/delete-product-async/{id}";
                var success = await _httpClient.DeleteAsync(url);
                if (!success)
                {
                    _logger.LogWarning("Xóa sản phẩm thất bại với Id {Id}", id);
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Xóa sản phẩm thất bại"
                    };
                }

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Xóa sản phẩm thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa sản phẩm {Id}: {Message}", id, ex.Message);
                return new BaseResponse
                { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<List<BestSellerItemDTO>> GetWeeklyBestSellersAsync(int take = 10)
        {
            try
            {
                var url = $"/api/Products/best-sellers-week?take={take}";
                var response = await _httpClient.GetAsync<List<BestSellerItemDTO>>(url);
                return response ?? new List<BestSellerItemDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi lấy sản phẩm bán chạy: {Message}", ex.Message);
                return new List<BestSellerItemDTO>();
            }
        }

        public async Task<List<ProductResponseDTO>> GetByIdsAsync(List<Guid> ids)
        {
            try
            {
                var url = "/api/Products/by-ids";
                var response = await _httpClient.PostAsync<List<Guid>, List<ProductResponseDTO>>(url, ids);
                return response ?? new List<ProductResponseDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi lấy danh sách sản phẩm theo IDs: {Message}", ex.Message);
                return new List<ProductResponseDTO>();
            }
        }

        private MultipartFormDataContent ConvertToFormData(CreateOrUpdateProductDTO product)
        {
            var formData = new MultipartFormDataContent();
            // Basic fields
            formData.Add(new StringContent(product.Id.HasValue ? product.Id.Value.ToString() : ""), "Id");
            formData.Add(new StringContent(string.IsNullOrEmpty(product.Name) ? "" : product.Name), "Name");
            formData.Add(new StringContent(string.IsNullOrEmpty(product.Description) ? "" : product.Description), "Description");
            formData.Add(new StringContent(product.BrandId.ToString()), "BrandId");
            if (product.SeasonIds.Any())
            {
                foreach (var id in product.SeasonIds)
                    formData.Add(new StringContent(id.ToString()), "SeasonIds");
            }

            if (product.MaterialIds.Any())
            {
                foreach (var id in product.MaterialIds)
                    formData.Add(new StringContent(id.ToString()), "MaterialIds");
            }

            if (product.CategoryIds.Any())
            {
                foreach (var id in product.CategoryIds)
                    formData.Add(new StringContent(id.ToString()), "CategoryIds");
            }

            if (product.ProductVariants.Any())
            {
                for (int i = 0; i < product.ProductVariants.Count; i++)
                {
                    var variant = product.ProductVariants[i];

                    if (variant.Id.HasValue)
                    {
                        formData.Add(new StringContent(variant.Id.Value.ToString()),
                            $"ProductVariants[{i}].Id");
                    }
                    if (variant.ProductId.HasValue)
                    {
                        formData.Add(new StringContent(variant.ProductId.Value.ToString()),
                            $"ProductVariants[{i}].ProductId");
                    }
                    formData.Add(new StringContent(variant.SizeId.ToString()), $"ProductVariants[{i}].SizeId");
                    formData.Add(new StringContent(String.IsNullOrEmpty(variant.Sku) ? "" : variant.Sku), $"ProductVariants[{i}].Sku");
                    formData.Add(new StringContent(String.IsNullOrEmpty(variant.ColourName) ? "" : variant.ColourName), $"ProductVariants[{i}].ColourName");
                    formData.Add(new StringContent(String.IsNullOrEmpty(variant.SizeName) ? "" : variant.SizeName), $"ProductVariants[{i}].SizeName");
                    formData.Add(new StringContent(variant.ColourId.ToString()), $"ProductVariants[{i}].ColourId");
                    formData.Add(new StringContent(variant.Price.ToString(CultureInfo.InvariantCulture)), $"ProductVariants[{i}].Price");
                    formData.Add(new StringContent((variant.Discount ?? 0).ToString(CultureInfo.InvariantCulture)), $"ProductVariants[{i}].Discount");

                    formData.Add(new StringContent(variant.OutOfStock.ToString()), $"ProductVariants[{i}].OutOfStock");
                    if (variant.MaxBuyPerOrder.HasValue)
                    {
                        formData.Add(new StringContent( variant.MaxBuyPerOrder.Value.ToString()), $"ProductVariants[{i}].MaxBuyPerOrder");
                    }
                    formData.Add(new StringContent(variant.Weight.ToString()), $"ProductVariants[{i}].Weight");
                    formData.Add(new StringContent(variant.Width.ToString()), $"ProductVariants[{i}].Width");
                    formData.Add(new StringContent(variant.Height.ToString()), $"ProductVariants[{i}].Height");
                    formData.Add(new StringContent(variant.Length.ToString()), $"ProductVariants[{i}].Length");
                    formData.Add(new StringContent(variant.InventoryQuantity.ToString()), $"ProductVariants[{i}].InventoryQuantity");
                    formData.Add(new StringContent(variant.StockHeight.ToString()),
                        $"ProductVariants[{i}].StockHeight");
                    formData.Add(new StringContent(((int)variant.ClosureType).ToString()),
                        $"ProductVariants[{i}].ClosureType");
                    formData.Add(new StringContent(variant.AllowReturn.ToString()),
                        $"ProductVariants[{i}].AllowReturn");
                    formData.Add(new StringContent(variant.Status.ToString()), $"ProductVariants[{i}].Status");
                    formData.Add(new StringContent("0"), $"ProductVariants[{i}].ViewNumber");
                    formData.Add(new StringContent("0"), $"ProductVariants[{i}].SellNumber");
                }
            }

            // Media Uploads
            if (product.MediaUploads != null && product.MediaUploads.Any())
            {
                for (int i = 0; i < product.MediaUploads.Count; i++)
                {
                    var media = product.MediaUploads[i];

                    if (media.Id.HasValue)
                        formData.Add(new StringContent(media.Id.Value.ToString()), $"MediaUploads[{i}].Id");
                    if (media.UploadFile != null)
                    {
                        var streamContent = new StreamContent(media.UploadFile.OpenReadStream());
                        streamContent.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue(media.UploadFile.ContentType);
                        formData.Add(streamContent, $"MediaUploads[{i}].UploadFile", media.UploadFile.FileName);
                    }

                    formData.Add(new StringContent(""), $"MediaUploads[{i}].ImageUrl");
                    formData.Add(new StringContent(""), $"MediaUploads[{i}].Base64Data");
                    formData.Add(new StringContent(""), $"MediaUploads[{i}].FileName");
                    formData.Add(new StringContent(""), $"MediaUploads[{i}].ContentType");
                }
            }

            return formData;
        }

        public async Task<Dictionary<Guid, List<ProductResponseDTO>>> GetHeaderProductsAsync()
        {
            try
            {
                var url = "/api/Products/get-header-products";
                var response = await _httpClient.GetAsync<Dictionary<Guid, List<ProductResponseDTO>>>(url);
                return response ?? new Dictionary<Guid, List<ProductResponseDTO>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi khi lấy sản phẩm cho header: {Message}", ex.Message);
                return new Dictionary<Guid, List<ProductResponseDTO>>();
            }
        }
    }
}