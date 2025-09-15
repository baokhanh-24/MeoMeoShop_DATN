using MeoMeo.Application.IServices;
using MeoMeo.Contract.DTOs.Statistics;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeoMeo.Contract.Commons;

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
            try
            {
                var revenue = await GetRevenueStatisticsAsync(request);
                var orders = await GetOrderStatisticsAsync(request);
                var customers = await GetCustomerStatisticsAsync(request);
                var inventory = await GetInventoryStatisticsAsync();

                var revenueByPeriod = await GetRevenueByPeriodAsync(request);
                var topProducts = await GetTopProductsAsync(request);
                var topProductsThisWeek = await GetTopProductsByPeriodAsync(new TopProductsRequestDTO { Period = StatisticsPeriod.Weekly });
                var topProductsThisMonth = await GetTopProductsByPeriodAsync(new TopProductsRequestDTO { Period = StatisticsPeriod.Monthly });
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
            catch (Exception ex)
            {
                return new DashboardStatisticsDTO()
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<RevenueStatisticsDTO> GetRevenueStatisticsAsync(StatisticsRequestDTO request)
        {
            try
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
            catch (Exception ex)
            {
                return new RevenueStatisticsDTO()
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<OrderStatisticsDTO> GetOrderStatisticsAsync(StatisticsRequestDTO request)
        {
            try
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
            catch (Exception ex)
            {
                return new OrderStatisticsDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message,
                };
            }
        }

        public async Task<CustomerStatisticsDTO> GetCustomerStatisticsAsync(StatisticsRequestDTO request)
        {
            try
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
            catch (Exception ex)
            {
                return new CustomerStatisticsDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message,
                };
            }
        }

        public async Task<InventoryStatisticsDTO> GetInventoryStatisticsAsync()
        {
            try
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
                var lowStockProducts =
                    inventoryData.Count(i => i.TotalQuantity <= lowStockThreshold && i.TotalQuantity > 0);
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
                var stockByProductData = await _inventoryRepository.Query()
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
                        (inv, pd) => new { inv.TotalQuantity, pd })
                    .Join(_productRepository.Query(),
                        x => x.pd.ProductId,
                        p => p.Id,
                        (x, p) => new
                        {
                            ProductName = p.Name,
                            SizeValue = x.pd.Size.Value,
                            ColourName = x.pd.Colour.Name,
                            StockQuantity = x.TotalQuantity,
                            LowStockThreshold = lowStockThreshold
                        })
                    .OrderByDescending(x => x.StockQuantity)
                    .Take(10)
                    .ToListAsync();

                var stockByProduct = stockByProductData.Select(x => new StockByProductDTO
                {
                    ProductName = x.ProductName,
                    SizeValue = x.SizeValue,
                    ColourName = x.ColourName,
                    ProductVariant = $"{x.ProductName} - Size: {x.SizeValue} - Màu sắc: {x.ColourName}",
                    StockQuantity = x.StockQuantity,
                    LowStockThreshold = x.LowStockThreshold
                }).ToList();

                return new InventoryStatisticsDTO
                {
                    TotalProducts = products,
                    LowStockProducts = lowStockProducts,
                    OutOfStockProducts = outOfStockProducts,
                    TotalInventoryValue = inventoryValue,
                    StockByProduct = stockByProduct
                };
            }
            catch (Exception ex)
            {
                return new InventoryStatisticsDTO()
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message,
                };
            }
        }

        private async Task<List<RevenueByPeriodDTO>> GetRevenueByPeriodAsync(StatisticsRequestDTO request)
        {
            var completedOrders = _orderRepository.Query()
                .Where(o => o.Status == EOrderStatus.Completed);

            if (request.Period == StatisticsPeriod.Daily)
            {
                var startDate = request.StartDate ?? DateTime.Now.AddDays(-30);
                var endDate = request.EndDate ?? DateTime.Now;

                var revenueByPeriod = await completedOrders
                    .Where(o => o.CreationTime >= startDate && o.CreationTime <= endDate)
                    .GroupBy(o => o.CreationTime.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.TotalPrice)
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                return revenueByPeriod.Select(x => new RevenueByPeriodDTO
                {
                    Period = x.Date.ToString("dd/MM/yyyy"),
                    Revenue = x.Revenue,
                    Date = x.Date
                }).ToList();
            }
            else if (request.Period == StatisticsPeriod.Monthly)
            {
                var startDate = request.StartDate ?? DateTime.Now.AddMonths(-12);
                var endDate = request.EndDate ?? DateTime.Now;

                var revenueByPeriod = await completedOrders
                    .Where(o => o.CreationTime >= startDate && o.CreationTime <= endDate)
                    .GroupBy(o => new { o.CreationTime.Year, o.CreationTime.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(o => o.TotalPrice)
                    })
                    .OrderBy(x => new DateTime(x.Year, x.Month, 1))
                    .ToListAsync();

                return revenueByPeriod.Select(x => new RevenueByPeriodDTO
                {
                    Period = $"{x.Month:00}/{x.Year}",
                    Revenue = x.Revenue,
                    Date = new DateTime(x.Year, x.Month, 1)
                }).ToList();
            }

            return new List<RevenueByPeriodDTO>();
        }

        private async Task<List<TopProductDTO>> GetTopProductsAsync(StatisticsRequestDTO request)
        {
            var startDate = request.StartDate ?? DateTime.Now.AddMonths(-1);
            var endDate = request.EndDate ?? DateTime.Now;

            var topProducts = await _orderDetailRepository.Query()
                .Where(od => od.Order.Status == EOrderStatus.Completed &&
                           od.Order.CreationTime >= startDate &&
                           od.Order.CreationTime <= endDate)
                .Join(_productDetailRepository.Query(),
                    od => od.ProductDetailId,
                    pd => pd.Id,
                    (od, pd) => new { od, pd })
                .Join(_productRepository.Query(),
                    x => x.pd.ProductId,
                    p => p.Id,
                    (x, p) => new { x.od, x.pd, p })
                .GroupBy(x => new { ProductName = x.p.Name, SizeValue = x.pd.Size.Value, ColourName = x.pd.Colour.Name })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    SizeValue = g.Key.SizeValue,
                    ColourName = g.Key.ColourName,
                    QuantitySold = g.Sum(x => x.od.Quantity),
                    Revenue = g.Sum(x => (decimal)x.od.Price * x.od.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();

            return topProducts.Select(x => new TopProductDTO
            {
                ProductName = x.ProductName,
                SizeValue = x.SizeValue,
                ColourName = x.ColourName,
                ProductVariant = $"{x.ProductName} - Size: {x.SizeValue} - Màu sắc: {x.ColourName}",
                QuantitySold = x.QuantitySold,
                Revenue = x.Revenue
            }).ToList();
        }

        private async Task<List<OrdersByHourDTO>> GetOrdersByHourAsync(StatisticsRequestDTO request)
        {
            var startDate = request.StartDate ?? DateTime.Now.Date;
            var endDate = request.EndDate ?? DateTime.Now.Date.AddDays(1);

            var ordersByHour = await _orderRepository.Query()
                .Where(o => o.CreationTime >= startDate && o.CreationTime < endDate)
                .GroupBy(o => o.CreationTime.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Where(o => o.Status == EOrderStatus.Completed).Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Hour)
                .ToListAsync();

            return ordersByHour.Select(x => new OrdersByHourDTO
            {
                Hour = $"{x.Hour:00}h",
                OrderCount = x.OrderCount,
                Revenue = x.Revenue
            }).ToList();
        }

        public async Task<List<TopProductDTO>> GetTopProductsByPeriodAsync(TopProductsRequestDTO request)
        {
            var now = DateTime.Now;
            DateTime periodStartDate;
            DateTime periodEndDate;

            switch (request.Period)
            {
                case StatisticsPeriod.Weekly:
                    // Lấy từ đầu tuần (thứ 2) đến cuối tuần (chủ nhật)
                    var daysFromMonday = (int)now.DayOfWeek - 1;
                    if (daysFromMonday == -1) daysFromMonday = 6; // Chủ nhật = 6 ngày từ thứ 2
                    periodStartDate = request.StartDate ?? now.Date.AddDays(-daysFromMonday);
                    periodEndDate = request.EndDate ?? periodStartDate.AddDays(7);
                    break;

                case StatisticsPeriod.Monthly:
                    periodStartDate = request.StartDate ?? new DateTime(now.Year, now.Month, 1);
                    periodEndDate = request.EndDate ?? periodStartDate.AddMonths(1);
                    break;

                case StatisticsPeriod.Yearly:
                    periodStartDate = request.StartDate ?? new DateTime(now.Year, 1, 1);
                    periodEndDate = request.EndDate ?? periodStartDate.AddYears(1);
                    break;

                case StatisticsPeriod.Daily:
                    periodStartDate = request.StartDate ?? now.Date;
                    periodEndDate = request.EndDate ?? now.Date.AddDays(1);
                    break;

                case StatisticsPeriod.Custom:
                    periodStartDate = request.StartDate ?? now.AddDays(-30);
                    periodEndDate = request.EndDate ?? now;
                    break;

                default:
                    periodStartDate = request.StartDate ?? now.AddDays(-30);
                    periodEndDate = request.EndDate ?? now;
                    break;
            }

            var topProducts = await _orderDetailRepository.Query()
                .Where(od => od.Order.Status == EOrderStatus.Completed &&
                           od.Order.CreationTime >= periodStartDate &&
                           od.Order.CreationTime < periodEndDate)
                .Join(_productDetailRepository.Query(),
                    od => od.ProductDetailId,
                    pd => pd.Id,
                    (od, pd) => new { od, pd })
                .Join(_productRepository.Query(),
                    x => x.pd.ProductId,
                    p => p.Id,
                    (x, p) => new { x.od, x.pd, p })
                .GroupBy(x => new { ProductName = x.p.Name, SizeValue = x.pd.Size.Value, ColourName = x.pd.Colour.Name })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    SizeValue = g.Key.SizeValue,
                    ColourName = g.Key.ColourName,
                    QuantitySold = g.Sum(x => x.od.Quantity),
                    Revenue = g.Sum(x => (decimal)x.od.Price * x.od.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToListAsync();

            return topProducts.Select(x => new TopProductDTO
            {
                ProductName = x.ProductName,
                SizeValue = x.SizeValue,
                ColourName = x.ColourName,
                ProductVariant = $"{x.ProductName} - Size: {x.SizeValue} - Màu sắc: {x.ColourName}",
                QuantitySold = x.QuantitySold,
                Revenue = x.Revenue
            }).ToList();
        }
    }
}
