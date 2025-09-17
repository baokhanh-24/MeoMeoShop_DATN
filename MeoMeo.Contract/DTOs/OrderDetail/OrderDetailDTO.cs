namespace MeoMeo.Contract.DTOs.OrderDetail;

public class OrderDetailDTO
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductDetailId { get; set; }
    public Guid PromotionDetailId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public float Price { get; set; }
    public float OriginalPrice { get; set; }
    public int Quantity { get; set; }

    public float GrandTotal { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public float Discount { get; set; }
    public string Image { get; set; } = string.Empty;

    // Thêm thông tin size và color
    public string SizeName { get; set; } = string.Empty;
    public string ColourName { get; set; } = string.Empty;
}