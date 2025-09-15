using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs
{
    public class InventoryBatchDTO
    {
        public Guid Id { get; set; }
        public Guid ImportBatchId { get; set; }
        public Guid ProductDetailId { get; set; }
        public float OriginalPrice { get; set; }
        public int Quantity { get; set; }
        public EInventoryBatchStatus Status { get; set; }
    }
}
