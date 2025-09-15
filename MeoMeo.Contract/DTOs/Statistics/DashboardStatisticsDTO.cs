using System;
using System.Collections.Generic;
using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Statistics
{
    public class DashboardStatisticsDTO : BaseResponse
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

    public class RevenueStatisticsDTO : BaseResponse
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal ThisYearRevenue { get; set; }
        public decimal GrowthPercentage { get; set; }
    }

    public class OrderStatisticsDTO : BaseResponse
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

    public class CustomerStatisticsDTO : BaseResponse
    {
        public int TotalCustomers { get; set; }
        public int NewCustomersToday { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomers { get; set; }
    }

    public class InventoryStatisticsDTO : BaseResponse
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
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
        public string SizeValue { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
        public string ProductVariant { get; set; } = string.Empty; // Format: "ProductName - SizeValue - ColourName"
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class StockByProductDTO : BaseResponse
    {
        public string ProductName { get; set; } = string.Empty;
        public string SizeValue { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
        public string ProductVariant { get; set; } = string.Empty; // Format: "ProductName - SizeValue - ColourName"
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
    }

    public class OrdersByHourDTO : BaseResponse
    {
        public string Hour { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
