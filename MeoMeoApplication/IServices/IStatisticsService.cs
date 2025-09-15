using MeoMeo.Contract.DTOs.Statistics;
using System;
using System.Threading.Tasks;

namespace MeoMeo.Application.IServices
{
    public interface IStatisticsService
    {
        Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(StatisticsRequestDTO request);
        Task<RevenueStatisticsDTO> GetRevenueStatisticsAsync(StatisticsRequestDTO request);
        Task<OrderStatisticsDTO> GetOrderStatisticsAsync(StatisticsRequestDTO request);
        Task<CustomerStatisticsDTO> GetCustomerStatisticsAsync(StatisticsRequestDTO request);
        Task<InventoryStatisticsDTO> GetInventoryStatisticsAsync();
        Task<List<TopProductDTO>> GetTopProductsByPeriodAsync(TopProductsRequestDTO request);
    }
}
