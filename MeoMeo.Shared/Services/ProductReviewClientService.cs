using MeoMeo.Shared.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;
using MeoMeo.Shared.Utilities;
using MeoMeo.Contract.DTOs.ProductReview;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Shared.Services
{
    public class ProductReviewClientService : IProductReviewClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<ProductReviewClientService> _logger;

        public ProductReviewClientService(IApiCaller httpClient, ILogger<ProductReviewClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public Task<BaseResponse> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductReviewDTO>> GetAllAsync()
        {
            try
            {
                var url = "/api/ProductReviews";
                var response = await _httpClient.GetAsync<List<ProductReviewDTO>>(url);
                return response ?? new List<ProductReviewDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product reviews: {Message}", ex.Message);
                return new List<ProductReviewDTO>();
            }
        }

        public async Task<PagingExtensions.PagedResult<ProductReviewDTO>> GetByProductDetailIdsAsync(
            GetListProductReviewDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"/api/ProductReviews/product-detail?{queryString}";
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<ProductReviewDTO>>(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product reviews by product detail id: {Message}", ex.Message);
                return new PagingExtensions.PagedResult<ProductReviewDTO>();
            }
        }

        private MultipartFormDataContent ConvertReviewToFormData(ProductReviewCreateOrUpdateDTO review)
        {
            var formData = new MultipartFormDataContent();

            // Basic fields
            if (review.Id.HasValue)
                formData.Add(new StringContent(review.Id.Value.ToString()), "Id");

            formData.Add(new StringContent(review.ProductDetailId.ToString()), "ProductDetailId");

            formData.Add(new StringContent(review.OrderId.ToString()), "OrderId");

            if (review.CustomerId.HasValue && review.CustomerId != Guid.Empty)
                formData.Add(new StringContent(review.CustomerId.Value.ToString()), "CustomerId");

            formData.Add(new StringContent(review.Rating.ToString(CultureInfo.InvariantCulture)), "Rating");

            formData.Add(new StringContent(string.IsNullOrEmpty(review.Content) ? "" : review.Content), "Content");

            // Files
            if (review.MediaUploads != null && review.MediaUploads.Any())
            {
                for (int i = 0; i < review.MediaUploads.Count; i++)
                {
                    var file = review.MediaUploads[i];
                    if (file.Id.HasValue)
                        formData.Add(new StringContent(file.Id.Value.ToString()), $"MediaUploads[{i}].Id");

                    if (file.UploadFile != null)
                    {
                        var streamContent = new StreamContent(file.UploadFile.OpenReadStream());
                        streamContent.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue(file.UploadFile.ContentType);
                        formData.Add(streamContent, $"MediaUploads[{i}].UploadFile", file.UploadFile.FileName);
                        formData.Add(new StringContent(""), $"MediaUploads[{i}].FileName");
                        formData.Add(new StringContent(""), $"MediaUploads[{i}].ContentType");
                    }
                }
            }

            return formData;
        }

        public async Task<BaseResponse> CreateAsync(ProductReviewCreateOrUpdateDTO dto)
        {
            try
            {
                var url = "/api/ProductReviews";
                var formData = ConvertReviewToFormData(dto);
                var result = await _httpClient.PostFormAsync<ProductReviewDTO>(url, formData);
                return result ?? new BaseResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product review: {Message}", ex.Message);
                throw;
            }
        }

        public Task<BaseResponse> UpdateAsync(ProductReviewCreateOrUpdateDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderItemForReviewDTO>> GetUnreviewedOrderItemsAsync()
        {
            try
            {
                var url = "/api/ProductReviews/unreviewed-items";
                var response = await _httpClient.GetAsync<List<OrderItemForReviewDTO>>(url);
                return response ?? new List<OrderItemForReviewDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách sản phẩm chưa đánh giá: {Message}", ex.Message);
                return new List<OrderItemForReviewDTO>();
            }
        }

        public async Task<List<ProductReviewDTO>> GetMyReviewsAsync()
        {
            try
            {
                var url = "/api/ProductReviews/my-reviews";
                var response = await _httpClient.GetAsync<List<ProductReviewDTO>>(url);
                return response ?? new List<ProductReviewDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách đánh giá của tôi: {Message}", ex.Message);
                return new List<ProductReviewDTO>();
            }
        }
    }
}