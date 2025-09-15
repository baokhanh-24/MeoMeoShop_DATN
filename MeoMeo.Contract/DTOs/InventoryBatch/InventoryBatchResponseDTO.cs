using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;
using System;

namespace MeoMeo.Contract.DTOs.InventoryBatch
{
    public class InventoryBatchResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid ImportBatchId { get; set; }
        public Guid ProductDetailId { get; set; }
        public float OriginalPrice { get; set; }
        public int Quantity { get; set; }
        public EInventoryBatchStatus Status { get; set; }
    }
}
