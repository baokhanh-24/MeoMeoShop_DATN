using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.ProductDetail
{
    public class GetListProductDetailRequestDTO :  BasePaging
    {
        public string? ProductNameFilter { get; set; }
        public float? PriceFilter { get; set; }
        public string? SKUFilter { get; set; }
        public float? StockHeightFilter { get; set; }
        public EClosureType? ClosureTypeFilter { get; set; }
        public int? OutOfStockFilter { get; set; }
        public EProductSortField SortField { get; set; } = EProductSortField.CreationTime;
        public ESortDirection SortDirection { get; set; } = ESortDirection.Desc;
    }
}
