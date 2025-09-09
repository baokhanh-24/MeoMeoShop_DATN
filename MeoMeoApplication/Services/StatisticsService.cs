using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Statistics;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductsDetailRepository _productDetailRepository;
        private readonly IIventoryBatchReposiory _inventoryRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public StatisticsService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IProductsDetailRepository productDetailRepository,
            IIventoryBatchReposiory inventoryRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
            _inventoryRepository = inventoryRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(StatisticsRequestDTO request)
        {
            var revenue = await GetRevenueStatisticsAsync(request);
            var orders = await GetOrderStatisticsAsync(request);
            var customers = await GetCustomerStatisticsAsync(request);
            var inventory = await GetInventoryStatisticsAsync();

            var revenueByPeriod = await GetRevenueByPeriodAsync(request);
            var topProducts = await GetTopProductsAsync(request);
            var topProductsThisWeek = await GetTopProductsByPeriodAsync(StatisticsPeriod.Weekly);
            var topProductsThisMonth = await GetTopProductsByPeriodAsync(StatisticsPeriod.Monthly);
            var ordersByHour = await GetOrdersByHourAsync(request);

            return new DashboardStatisticsDTO
            {
                Revenue = revenue,
                Orders = orders,
                Customers = customers,
                Inventory = inventory,
                RevenueByPeriod = revenueByPeriod,
                TopProducts = topProducts,
                TopProductsThisWeek = topProductsThisWeek,
                TopProductsThisMonth = topProductsThisMonth,
                OrdersByHour = ordersByHour
            };
        }

        public async Task<RevenueStatisticsDTO> GetRevenueStatisticsAsync(StatisticsRequestDTO request)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var thisMonth = new DateTime(now.Year, now.Month, 1);
            var thisYear = new DateTime(now.Year, 1, 1);

            var completedOrders = _orderRepository.Query()
                .Where(o => o.Status == EOrderStatus.Completed);

            var totalRevenue = await completedOrders.SumAsync(o => o.TotalPrice);
            var todayRevenue = await completedOrders
                .Where(o => o.CreationTime >= today && o.CreationTime < today.AddDays(1))
                .SumAsync(o => o.TotalPrice);
            var thisMonthRevenue = await completedOrders
                .Where(o => o.CreationTime >= thisMonth)
                .SumAsync(o => o.TotalPrice);
            var thisYearRevenue = await completedOrders
                .Where(o => o.CreationTime >= thisYear)
                .SumAsync(o => o.TotalPrice);

            // Calculate growth percentage (this month vs last month)
            var lastMonth = thisMonth.AddMonths(-1);
            var lastMonthRevenue = await completedOrders
                .Where(o => o.CreationTime >= lastMonth && o.CreationTime < thisMonth)
                .SumAsync(o => o.TotalPrice);

            var growthPercentage = lastMonthRevenue > 0
                ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100
                : 0;

            return new RevenueStatisticsDTO
            {
                TotalRevenue = totalRevenue,
                TodayRevenue = todayRevenue,
                ThisMonthRevenue = thisMonthRevenue,
                ThisYearRevenue = thisYearRevenue,
                GrowthPercentage = growthPercentage
            };
        }

        public async Task<OrderStatisticsDTO> GetOrderStatisticsAsync(StatisticsRequestDTO request)
        {
            var now = DateTime.Now;
            var today = now.Date;

            var orders = _orderRepository.Query();

            var totalOrders = await orders.CountAsync();
            var todayOrders = await orders
                .Where(o => o.CreationTime >= today && o.CreationTime < today.AddDays(1))
                .CountAsync();
            var posOrders = await orders.Where(o => o.Type == EOrderType.Store).CountAsync();
            var onlineOrders = await orders.Where(o => o.Type == EOrderType.Online).CountAsync();
            var completedOrders = await orders.Where(o => o.Status == EOrderStatus.Completed).CountAsync();
            var cancelledOrders = await orders.Where(o => o.Status == EOrderStatus.Canceled).CountAsync();
            var pendingOrders = await orders.Where(o => o.Status == EOrderStatus.Pending).CountAsync();
            var confirmedOrders = await orders.Where(o => o.Status == EOrderStatus.Confirmed).CountAsync();
            var inTransitOrders = await orders.Where(o => o.Status == EOrderStatus.InTransit).CountAsync();

            return new OrderStatisticsDTO
            {
                TotalOrders = totalOrders,
                TodayOrders = todayOrders,
                PosOrders = posOrders,
                OnlineOrders = onlineOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                PendingOrders = pendingOrders,
                ConfirmedOrders = confirmedOrders,
                InTransitOrders = inTransitOrders
            };
        }

        public async Task<CustomerStatisticsDTO> GetCustomerStatisticsAsync(StatisticsRequestDTO request)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var thisMonth = new DateTime(now.Year, now.Month, 1);

            var customers = _customerRepository.Query();

            var totalCustomers = await customers.CountAsync();
            var newCustomersToday = await customers
                .Where(c => c.CreationTime >= today && c.CreationTime < today.AddDays(1))
                .CountAsync();
            var newCustomersThisMonth = await customers
                .Where(c => c.CreationTime >= thisMonth)
                .CountAsync();

            // Active customers (customers who made orders in last 30 days)
            var thirtyDaysAgo = now.AddDays(-30);
            var activeCustomers = await _orderRepository.Query()
                .Where(o => o.CreationTime >= thirtyDaysAgo)
                .Select(o => o.CustomerId)
                .Distinct()
                .CountAsync();

            return new CustomerStatisticsDTO
            {
                TotalCustomers = totalCustomers,
                NewCustomersToday = newCustomersToday,
                NewCustomersThisMonth = newCustomersThisMonth,
                ActiveCustomers = activeCustomers
            };
        }

        public async Task<InventoryStatisticsDTO> GetInventoryStatisticsAsync()
        {
            var products = await _productRepository.Query().CountAsync();

            // Get inventory data with product details
            var inventoryData = await _inventoryRepository.Query()
                .Where(i => i.Status == EInventoryBatchStatus.Approved)
                .GroupBy(i => i.ProductDetailId)
                .Select(g => new
                {
                    ProductDetailId = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity)
                })
                .ToListAsync();

            var lowStockThreshold = 10; // Define low stock threshold
            var lowStockProducts = inventoryData.Count(i => i.TotalQuantity <= lowStockThreshold && i.TotalQuantity > 0);
            var outOfStockProducts = inventoryData.Count(i => i.TotalQuantity <= 0);

            // Calculate total inventory value
            var inventoryValue = await _inventoryRepository.Query()
                .Where(i => i.Status == EInventoryBatchStatus.Approved && i.Quantity > 0)
                .Join(_productDetailRepository.Query(),
                    i => i.ProductDetailId,
                    pd => pd.Id,
                    (i, pd) => new { i.Quantity, pd.Price })
                .SumAsync(x => x.Quantity * (decimal)x.Price);

            // Get stock by product for chart
            var stockByProduct = await _inventoryRepository.Query()
                .Where(i => i.Status == EInventoryBatchStatus.Approved)
                .GroupBy(i => i.ProductDetailId)
                .Select(g => new
                {
                    ProductDetailId = g.Key,
                    TotalQuantity = g.Sum(i => i.Quantity)
                })
                .Join(_productDetailRepository.Query(),
                    inv => inv.ProductDetailId,
                    pd => pd.Id,
                    (inv, pd) => new { inv.TotalQuantity, pd.ProductId })
                .Join(_productRepository.Query(),
                    x => x.ProductId,
                    p => p.Id,
                    (x, p) => new StockByProductDTO
                    {
                        ProductName = p.Name,
                        StockQuantity = x.TotalQuantity,
                        LowStockThreshold = lowStockThreshold
                    })
                .OrderByDescending(x => x.StockQuantity)
                .Take(10)
                .ToListAsync();

            return new InventoryStatisticsDTO
            {
                TotalProducts = products,
                LowStockProducts = lowStockProducts,
                OutOfStockProducts = outOfStockProducts,
                TotalInventoryValue = (int)inventoryValue,
                StockByProduct = stockByProduct
            };
        }

        private async Task<List<RevenueByPeriodDTO>> GetRevenueByPeriodAsync(StatisticsRequestDTO request)
        {
            var completedOrders = _orderRepository.Query()
                .Where(o => o.Status == EOrderStatus.Completed);

            if (request.Period == StatisticsPeriod.Daily)
            {
                var startDate = request.StartDate ?? DateTime.Now.AddDays(-30);
                var endDate = request.EndDate ?? DateTime.Now;

                return await completedOrders
                    .Where(o => o.CreationTime >= startDate && o.CreationTime <= endDate)
                    .GroupBy(o => o.CreationTime.Date)
                    .Select(g => new RevenueByPeriodDTO
                    {
                        Period = g.Key.ToString("MM/dd"),
                        Revenue = g.Sum(o => o.TotalPrice),
                        Date = g.Key
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();
            }
            else if (request.Period == StatisticsPeriod.Monthly)
            {
                var startDate = request.StartDate ?? DateTime.Now.AddMonths(-12);
                var endDate = request.EndDate ?? DateTime.Now;

                return await completedOrders
                    .Where(o => o.CreationTime >= startDate && o.CreationTime <= endDate)
                    .GroupBy(o => new { o.CreationTime.Year, o.CreationTime.Month })
                    .Select(g => new RevenueByPeriodDTO
                    {
                        Period = $"{g.Key.Month:00}/{g.Key.Year}",
                        Revenue = g.Sum(o => o.TotalPrice),
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();
            }

            return new List<RevenueByPeriodDTO>();
        }

        private async Task<List<TopProductDTO>> GetTopProductsAsync(StatisticsRequestDTO request)
        {
            var startDate = request.StartDate ?? DateTime.Now.AddMonths(-1);
            var endDate = request.EndDate ?? DateTime.Now;

            return await _orderDetailRepository.Query()
                .Where(od => od.Order.Status == EOrderStatus.Completed &&
                           od.Order.CreationTime >= startDate &&
                           od.Order.CreationTime <= endDate)
                .GroupBy(od => od.ProductName)
                .Select(g => new TopProductDTO
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => (decimal)od.Price * od.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<OrdersByHourDTO>> GetOrdersByHourAsync(StatisticsRequestDTO request)
        {
            var startDate = request.StartDate ?? DateTime.Now.Date;
            var endDate = request.EndDate ?? DateTime.Now.Date.AddDays(1);

            return await _orderRepository.Query()
                .Where(o => o.CreationTime >= startDate && o.CreationTime < endDate)
                .GroupBy(o => o.CreationTime.Hour)
                .Select(g => new OrdersByHourDTO
                {
                    Hour = $"{g.Key:00}h",
                    OrderCount = g.Count(),
                    Revenue = g.Where(o => o.Status == EOrderStatus.Completed).Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Hour)
                .ToListAsync();
        }

        public async Task<List<TopProductDTO>> GetTopProductsByPeriodAsync(StatisticsPeriod period, DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.Now;
            DateTime periodStartDate;
            DateTime periodEndDate;

            switch (period)
            {
                case StatisticsPeriod.Weekly:
                    // Lấy từ đầu tuần (thứ 2) đến cuối tuần (chủ nhật)
                    var daysFromMonday = (int)now.DayOfWeek - 1;
                    if (daysFromMonday == -1) daysFromMonday = 6; // Chủ nhật = 6 ngày từ thứ 2
                    periodStartDate = startDate ?? now.Date.AddDays(-daysFromMonday);
                    periodEndDate = endDate ?? periodStartDate.AddDays(7);
                    break;

                case StatisticsPeriod.Monthly:
                    periodStartDate = startDate ?? new DateTime(now.Year, now.Month, 1);
                    periodEndDate = endDate ?? periodStartDate.AddMonths(1);
                    break;

                case StatisticsPeriod.Yearly:
                    periodStartDate = startDate ?? new DateTime(now.Year, 1, 1);
                    periodEndDate = endDate ?? periodStartDate.AddYears(1);
                    break;

                case StatisticsPeriod.Daily:
                    periodStartDate = startDate ?? now.Date;
                    periodEndDate = endDate ?? now.Date.AddDays(1);
                    break;

                case StatisticsPeriod.Custom:
                    periodStartDate = startDate ?? now.AddDays(-30);
                    periodEndDate = endDate ?? now;
                    break;

                default:
                    periodStartDate = startDate ?? now.AddDays(-30);
                    periodEndDate = endDate ?? now;
                    break;
            }

            return await _orderDetailRepository.Query()
                .Where(od => od.Order.Status == EOrderStatus.Completed &&
                           od.Order.CreationTime >= periodStartDate &&
                           od.Order.CreationTime < periodEndDate)
                .GroupBy(od => od.ProductName)
                .Select(g => new TopProductDTO
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => (decimal)od.Price * od.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();
        }
    }
}
