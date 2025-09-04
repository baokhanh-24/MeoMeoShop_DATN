namespace MeoMeo.Contract.DTOs.OrderDetail;

public class OrderDetailDTO
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductDetailId { get; set; }
    public Guid PromotionDetailId { get; set; }
    public Guid? InventoryBatchId { get; set; }
    public string Sku { get; set; }
    public float Price { get; set; }
    public int Quantity { get; set; }
    
    public float GrandTotal { get; set; }
    public string ProductName { get; set; }
    public float Discount { get; set; }
    public string Note { get; set; }
    public string Image { get; set; }
}