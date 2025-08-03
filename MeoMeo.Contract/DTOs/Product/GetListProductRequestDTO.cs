using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Product
{
    public class GetListProductRequestDTO : BasePaging
    {
        public string? NameFilter { get; set; }
        public string? SKUFilter { get; set; }
        public int? MaxSellNumberFilter { get; set; }
        public int? MinSellNumberFilter { get; set; }
        public float? MinStockHeightFilter { get; set; }
        public float? MaxStockHeightFilter { get; set; }
        public Guid? BrandFilter { get; set; }
        public Guid? CategoryFilter { get; set; }
        public Guid? MaterialFilter { get; set; }
        public Guid? SeasonFilter { get; set; }
        public Guid? SizeFilter { get; set; }
        public Guid? ColourFilter { get; set; }
        public EClosureType? ClosureTypeFilter { get; set; }
        public decimal? MinPriceFilter { get; set; }
        public decimal? MaxPriceFilter { get; set; }
        public EProductStatus? StatusFilter { get; set; }
        public bool? AllowReturnFilter { get; set; }
        public EProductSortField SortField { get; set; } = EProductSortField.CreationTime;
        public ESortDirection SortDirection { get; set; } = ESortDirection.Desc;
    }


}
