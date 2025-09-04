using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CartResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid CustomersId { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
    public class CartWithDetailsResponseDTO : BaseResponse
    {
        public decimal TotalPrice { get; set; }
        public List<CartDetailItemDTO> Items { get; set; } = new();
    }

    public class CartDetailItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductDetailId { get; set; }
        public Guid PromotionDetailId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public string SizeName { get; set; }
        public string ColourName { get; set; }
        public string Thumbnail { get; set; }
        
        // Thông tin vận chuyển - cần thiết cho GHN
        public int Weight { get; set; } = 500; // Trọng lượng (gram)
        public int Length { get; set; } = 15;  // Chiều dài (cm)
        public int Width { get; set; } = 15;   // Chiều rộng (cm)
        public int Height { get; set; } = 15;  // Chiều cao (cm)
        
        // Giới hạn mua hàng
        public int? MaxBuyPerOrder { get; set; } // Số lượng được mua tối đa trên 1 đơn hàng
    }
}
