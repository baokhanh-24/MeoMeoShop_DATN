using MeoMeo.Contract.DTOs.Statistics;
using System.Threading.Tasks;

namespace MeoMeo.Shared.IServices
{
    public interface IStatisticsClientService
    {
        Task<DashboardStatisticsDTO?> GetDashboardStatisticsAsync(StatisticsRequestDTO request);
        Task<RevenueStatisticsDTO?> GetRevenueStatisticsAsync(StatisticsRequestDTO request);
        Task<OrderStatisticsDTO?> GetOrderStatisticsAsync(StatisticsRequestDTO request);
        Task<CustomerStatisticsDTO?> GetCustomerStatisticsAsync(StatisticsRequestDTO request);
        Task<InventoryStatisticsDTO?> GetInventoryStatisticsAsync();
        Task<List<TopProductDTO>?> GetTopProductsByPeriodAsync(StatisticsPeriod period, DateTime? startDate = null, DateTime? endDate = null);
    }
}
