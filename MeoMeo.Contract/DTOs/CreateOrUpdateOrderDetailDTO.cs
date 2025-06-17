using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateOrderDetailDTO
    {
        public Guid? Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid PromotionDetailId { get; set; }
        public Guid? InventoryBatchId { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public float Discount { get; set; }
        public string Note { get; set; }
        public string Image { get; set; }
        public EOrderStatus Status { get; set; }
    }
}
