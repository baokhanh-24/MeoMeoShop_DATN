using MeoMeo.Domain.Commons.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateProductDetailDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Barcode { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public EProductDetailGender Gender { get; set; }
        public float StockHeight { get; set; }
        public float ShoeLength { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int Status { get; set; }
    }
}
