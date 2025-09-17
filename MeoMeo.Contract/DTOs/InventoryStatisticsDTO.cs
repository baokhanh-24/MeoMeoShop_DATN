using System;
using System.Collections.Generic;
using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs
{
    public class InventoryStatisticsItemDTO
    {
        public Guid ProductDetailId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
        public string SizeValue { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int OutOfStockThreshold { get; set; }
        public string StockStatus { get; set; } = string.Empty; // "InStock", "LowStock", "OutOfStock"
        public DateTime LastUpdated { get; set; }
    }

    public class InventoryStatisticsSummaryDTO
    {
        public int TotalProducts { get; set; }
        public int ProductsInStock { get; set; }
        public int ProductsLowStock { get; set; }
        public int ProductsOutOfStock { get; set; }
    }

    public class GetInventoryStatisticsRequestDTO
    {
        public string? NameFilter { get; set; }
        public string? SKUFilter { get; set; }
        public string? StockStatusFilter { get; set; } // "InStock", "LowStock", "OutOfStock"
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetInventoryStatisticsResponseDTO : BaseResponse
    {
        public List<InventoryStatisticsItemDTO> Items { get; set; } = new List<InventoryStatisticsItemDTO>();
        public InventoryStatisticsSummaryDTO Summary { get; set; } = new InventoryStatisticsSummaryDTO();
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class InventoryHistoryItemDTO
    {
        public DateTime Date { get; set; }
        public EInventoryTranctionType TransactionType { get; set; }
        public int QuantityChange { get; set; } // Số dương cho Import, số âm cho Export
        public int StockAfter { get; set; }
        public string Note { get; set; } = string.Empty;
    }

    public class GetInventoryHistoryRequestDTO
    {
        public Guid ProductDetailId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetInventoryHistoryResponseDTO : BaseResponse
    {
        public List<InventoryHistoryItemDTO> Items { get; set; } = new List<InventoryHistoryItemDTO>();
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
