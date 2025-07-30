using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class CreateOrUpdateProductDetailResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        //public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string Sku { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public EProductDetailGender Gender { get; set; }
        public float StockHeight { get; set; }
        public EClosureType ClosureType { get; set; }
        public int OutOfStock { get; set; }
        public bool AllowReturn { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public int Status { get; set; }
    }
}
