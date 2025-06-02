using MeoMeo.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class ProductDetail : BaseEntity
    {
        public string Barcode { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int Gender { get; set; }
        public float StockHeight { get; set; }
        public float ShoeLength { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModification { get; set; }


        public Guid ProductId { get; set; }
    }
}
