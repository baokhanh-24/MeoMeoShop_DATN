using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class GetListProductDetailRequestDTO : BasePaging
    {
        public string? ProductNameFilter { get; set; }
        public float? PriceFilter { get; set; }
        public string? DescriptionFilter { get; set; }
        public string? SKUFilter { get; set; }
        public EProductDetailGender? GenderFilter { get; set; }
        public float? StockHeightFilter { get; set; }
        public float? ShoeLengthFilter { get; set; }
        public int? OutOfStockFilter { get; set; }
        
        public EProductDetailSortField? SortField { get; set; }
        public ESortDirection? SortDirection { get; set; }
    }
}
