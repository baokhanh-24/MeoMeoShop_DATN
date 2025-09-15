using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;

namespace MeoMeo.Contract.DTOs.ImportBatch
{
    public class ImportBatchDetailDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public EImportStatus Status { get; set; }

        // Danh sách sản phẩm trong lô nhập này
        public List<ImportBatchProductDTO> Products { get; set; } = new List<ImportBatchProductDTO>();
    }

    public class ImportBatchProductDTO
    {
        public Guid InventoryBatchId { get; set; }
        public Guid ProductDetailId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ColourName { get; set; } = string.Empty;
        public string SizeValue { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public float OriginalPrice { get; set; }
        public int Quantity { get; set; }
        public EInventoryBatchStatus Status { get; set; }
    }
}
