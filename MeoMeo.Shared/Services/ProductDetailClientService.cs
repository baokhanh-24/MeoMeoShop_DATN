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

        public async Task<PagingExtensions.PagedResult<ProductDetailDTO>> GetAllProductDetailAsync(
            GetListProductDetailRequestDTO request)
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

        public async Task<CreateOrUpdateProductDetailDTO> GetProductDetailByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/ProductDetails/find-product-detail-by-id-async/{id}";
                var response = await _httpClient.GetAsync<CreateOrUpdateProductDetailDTO>(url);
                return response ?? new CreateOrUpdateProductDetailDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy sản phẩm");
                return new CreateOrUpdateProductDetailDTO();
            }
        }

        public async Task<ProductDetailDetailDTO> GetByIdDetail(Guid id)
        {
            try
            {
                var url = $"/api/ProductDetails/get-by-id-detail/{id}";
                var response = await _httpClient.GetAsync<ProductDetailDetailDTO>(url);
                return response ?? new ProductDetailDetailDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy chi tiết sản phẩm");
                return new ProductDetailDetailDTO();
            }
        }

        public async Task<CreateOrUpdateProductDetailResponseDTO> CreateProductDetailAsync(
            CreateOrUpdateProductDetailDTO productDetail)
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
                return new CreateOrUpdateProductDetailResponseDTO
                    { ResponseStatus = BaseStatus.Error, Message = ex.Message };
            }
        }

        public async Task<CreateOrUpdateProductDetailResponseDTO> UpdateProductDetailAsync(
            CreateOrUpdateProductDetailDTO productDetail)
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
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật sản phẩm {Id}: {Message}", productDetail.Id,
                    ex.Message);
                return new CreateOrUpdateProductDetailResponseDTO
                    { ResponseStatus = BaseStatus.Error, Message = ex.Message };
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

            // Basic fields
            if (productDetail.Id.HasValue)
                formData.Add(new StringContent(productDetail.Id.Value.ToString()), "Id");

            if (productDetail.ProductId.HasValue)
                formData.Add(new StringContent(productDetail.ProductId.Value.ToString()), "ProductId");

            if (!string.IsNullOrEmpty(productDetail.ProductName))
                formData.Add(new StringContent(productDetail.ProductName), "ProductName");

            formData.Add(new StringContent(productDetail.Price.ToString()), "Price");
            formData.Add(new StringContent(productDetail.Description ?? ""), "Description");
            formData.Add(new StringContent(productDetail.StockHeight.ToString()), "StockHeight");
            formData.Add(new StringContent(((int)productDetail.ClosureType).ToString()), "ClosureType");
            formData.Add(new StringContent(productDetail.OutOfStock.ToString()), "OutOfStock");
            formData.Add(new StringContent(productDetail.AllowReturn.ToString()), "AllowReturn");
            formData.Add(new StringContent(productDetail.Status.ToString()), "Status");
            formData.Add(new StringContent(productDetail.BrandId.ToString()), "BrandId");

            foreach (var id in productDetail.SeasonIds)
                formData.Add(new StringContent(id.ToString()), "SeasonIds");

            foreach (var id in productDetail.MaterialIds)
                formData.Add(new StringContent(id.ToString()), "MaterialIds");

            foreach (var id in productDetail.CategoryIds)
                formData.Add(new StringContent(id.ToString()), "CategoryIds");

            // Images
            if (productDetail.MediaUploads != null && productDetail.MediaUploads.Any())
            {
                for (int i = 0; i < productDetail.MediaUploads.Count; i++)
                {
                    var image = productDetail.MediaUploads[i];

                    if (image.Id.HasValue)
                        formData.Add(new StringContent(image.Id.Value.ToString()), $"MediaUploads[{i}].Id");

                    if (image.UploadFile != null)
                    {
                        var streamContent = new StreamContent(image.UploadFile.OpenReadStream());
                        streamContent.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue(image.UploadFile.ContentType);
                        formData.Add(streamContent, $"MediaUploads[{i}].UploadFile", image.UploadFile.FileName);
                    }


                    formData.Add(new StringContent(""), $"MediaUploads[{i}].ImageUrl");
                    formData.Add(new StringContent(""), $"MediaUploads[{i}].Base64Data");
                    formData.Add(new StringContent(""), $"MediaUploads[{i}].FileName");
                    formData.Add(new StringContent(""), $"MediaUploads[{i}].ContentType");
                }
            }

            return formData;
        }
    }
}