using MeoMeo.Shared.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeoMeo.Shared.Utilities;

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

        public async Task<IEnumerable<ProductReviewDTO>> GetAllAsync()
        {
            try
            {
                var url = "/api/ProductReviews";
                var response = await _httpClient.GetAsync<IEnumerable<ProductReviewDTO>>(url);
                return response ?? Array.Empty<ProductReviewDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product reviews: {Message}", ex.Message);
                return Array.Empty<ProductReviewDTO>();
            }
        }
    }
}


