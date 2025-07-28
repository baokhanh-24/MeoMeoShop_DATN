using MeoMeo.Shared.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.ProductDetail;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.IO;

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
                var formData = ConvertToFormData(productDetail);
                var result = await _httpClient.PostFormAsync<CreateOrUpdateProductDetailResponseDTO>(url, formData);
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
                var formData = ConvertToFormData(productDetail);
                var result = await _httpClient.PutFormAsync<CreateOrUpdateProductDetailResponseDTO>(url, formData);
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

        private MultipartFormDataContent ConvertToFormData(CreateOrUpdateProductDetailDTO productDetail)
        {
            var formData = new MultipartFormDataContent();

            // Add basic properties
            if (productDetail.Id.HasValue)
                formData.Add(new StringContent(productDetail.Id.Value.ToString()), "Id");
            
            if (productDetail.ProductId.HasValue)
                formData.Add(new StringContent(productDetail.ProductId.Value.ToString()), "ProductId");
            
            if (!string.IsNullOrEmpty(productDetail.ProductName))
                formData.Add(new StringContent(productDetail.ProductName), "ProductName");
            
            formData.Add(new StringContent(productDetail.Price.ToString()), "Price");
            
            if (!string.IsNullOrEmpty(productDetail.Description))
                formData.Add(new StringContent(productDetail.Description), "Description");
            
            formData.Add(new StringContent(((int)productDetail.Gender).ToString()), "Gender");
            formData.Add(new StringContent(productDetail.StockHeight.ToString()), "StockHeight");
            formData.Add(new StringContent(productDetail.ShoeLength.ToString()), "ShoeLength");
            formData.Add(new StringContent(productDetail.OutOfStock.ToString()), "OutOfStock");
            formData.Add(new StringContent(productDetail.AllowReturn.ToString()), "AllowReturn");
            formData.Add(new StringContent(productDetail.Status.ToString()), "Status");
            formData.Add(new StringContent(productDetail.BrandId.ToString()), "BrandId");

            // Add collections as JSON strings
            if (productDetail.SizeIds != null && productDetail.SizeIds.Any())
                formData.Add(new StringContent(JsonSerializer.Serialize(productDetail.SizeIds)), "SizeIds");
            
            if (productDetail.ColourIds != null && productDetail.ColourIds.Any())
                formData.Add(new StringContent(JsonSerializer.Serialize(productDetail.ColourIds)), "ColourIds");
            
            if (productDetail.SeasonIds != null && productDetail.SeasonIds.Any())
                formData.Add(new StringContent(JsonSerializer.Serialize(productDetail.SeasonIds)), "SeasonIds");
            
            if (productDetail.MaterialIds != null && productDetail.MaterialIds.Any())
                formData.Add(new StringContent(JsonSerializer.Serialize(productDetail.MaterialIds)), "MaterialIds");
            
            if (productDetail.CategoryIds != null && productDetail.CategoryIds.Any())
                formData.Add(new StringContent(JsonSerializer.Serialize(productDetail.CategoryIds)), "CategoryIds");

            // Add images
            if (productDetail.Images != null && productDetail.Images.Any())
            {
                for (int i = 0; i < productDetail.Images.Count; i++)
                {
                    var image = productDetail.Images[i];
                    
                    if (image.Id.HasValue)
                        formData.Add(new StringContent(image.Id.Value.ToString()), $"Images[{i}].Id");
                    
                    if (image.UploadFile != null)
                    {
                        var streamContent = new StreamContent(image.UploadFile.OpenReadStream());
                        formData.Add(streamContent, $"Images[{i}].UploadFile", image.UploadFile.FileName);
                    }
                    
                    // if (!string.IsNullOrEmpty(image.ImageUrl))
                    //     formData.Add(new StringContent(image.ImageUrl), $"Images[{i}].ImageUrl");
                    
                    // if (!string.IsNullOrEmpty(image.Base64Data))
                    //     formData.Add(new StringContent(image.Base64Data), $"Images[{i}].Base64Data");
                    
                    // if (!string.IsNullOrEmpty(image.FileName))
                    //     formData.Add(new StringContent(image.FileName), $"Images[{i}].FileName");
                    
                    // if (!string.IsNullOrEmpty(image.ContentType))
                    //     formData.Add(new StringContent(image.ContentType), $"Images[{i}].ContentType");
                }
            }

            return formData;
        }
    }
}
