using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs.Statistics
{
    public class DashboardStatisticsDTO
    {
        public RevenueStatisticsDTO Revenue { get; set; } = new();
        public OrderStatisticsDTO Orders { get; set; } = new();
        public CustomerStatisticsDTO Customers { get; set; } = new();
        public InventoryStatisticsDTO Inventory { get; set; } = new();
        public List<RevenueByPeriodDTO> RevenueByPeriod { get; set; } = new();
        public List<TopProductDTO> TopProducts { get; set; } = new();
        public List<TopProductDTO> TopProductsThisWeek { get; set; } = new();
        public List<TopProductDTO> TopProductsThisMonth { get; set; } = new();
        public List<OrdersByHourDTO> OrdersByHour { get; set; } = new();
    }

    public class RevenueStatisticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal ThisYearRevenue { get; set; }
        public decimal GrowthPercentage { get; set; }
    }

    public class OrderStatisticsDTO
    {
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public int PosOrders { get; set; }
        public int OnlineOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int InTransitOrders { get; set; }
    }

    public class CustomerStatisticsDTO
    {
        public int TotalCustomers { get; set; }
        public int NewCustomersToday { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomers { get; set; }
    }

    public class InventoryStatisticsDTO
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalInventoryValue { get; set; }
        public List<StockByProductDTO> StockByProduct { get; set; } = new();
    }

    public class RevenueByPeriodDTO
    {
        public string Period { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public DateTime Date { get; set; }
    }

    public class TopProductDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class StockByProductDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
    }

    public class OrdersByHourDTO
    {
        public string Hour { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
