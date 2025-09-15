using MeoMeo.Contract.DTOs.Statistics;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeoMeo.Shared.Services
{
    public class StatisticsClientService : IStatisticsClientService
    {
        private readonly IApiCaller _apiCaller;

        public StatisticsClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<DashboardStatisticsDTO?> GetDashboardStatisticsAsync(StatisticsRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"api/Statistics/dashboard?{queryString}";
                var result = await _apiCaller.GetAsync<DashboardStatisticsDTO>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dashboard statistics: {ex.Message}");
                return null;
            }
        }

        public async Task<RevenueStatisticsDTO?> GetRevenueStatisticsAsync(StatisticsRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"api/Statistics/revenue?{queryString}";
                var result = await _apiCaller.GetAsync<RevenueStatisticsDTO>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting revenue statistics: {ex.Message}");
                return null;
            }
        }

        public async Task<OrderStatisticsDTO?> GetOrderStatisticsAsync(StatisticsRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"api/Statistics/orders?{queryString}";
                var result = await _apiCaller.GetAsync<OrderStatisticsDTO>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order statistics: {ex.Message}");
                return null;
            }
        }

        public async Task<CustomerStatisticsDTO?> GetCustomerStatisticsAsync(StatisticsRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"api/Statistics/customers?{queryString}";
                var result = await _apiCaller.GetAsync<CustomerStatisticsDTO>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer statistics: {ex.Message}");
                return null;
            }
        }

        public async Task<InventoryStatisticsDTO?> GetInventoryStatisticsAsync()
        {
            try
            {
                var url = "api/Statistics/inventory";
                var result = await _apiCaller.GetAsync<InventoryStatisticsDTO>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting inventory statistics: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TopProductDTO>?> GetTopProductsByPeriodAsync(TopProductsRequestDTO request)
        {
            try
            {
                var queryString = BuildQuery.ToQueryString(request);
                var url = $"api/Statistics/top-products?{queryString}";
                var result = await _apiCaller.GetAsync<List<TopProductDTO>>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting top products by period: {ex.Message}");
                return null;
            }
        }
    }
}
