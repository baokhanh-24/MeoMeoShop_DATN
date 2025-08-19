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
    }
}
