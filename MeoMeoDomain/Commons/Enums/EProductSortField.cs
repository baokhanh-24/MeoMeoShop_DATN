using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums;

public enum EProductSortField
{
    [Display(Name = "Tên")]
    Name, // sắp xếp theo tên
    [Display(Name = "Giá bán")]
    Price,  // sắp xếp theo giá
    [Display(Name = "% Giảm giá")]
    Discount,  // sắp xếp % giảm
    [Display(Name = "Ngày tạo")]
    CreationTime,  // hàng mới về
    [Display(Name = "Số lượng tồn kho")]
    StockQuantity , // sắp xếp theo số lượng tồn,
    [Display(Name = "Số lượt xem")]
    ViewNumber,  // sắp xếp theo số lượt xem,
    [Display(Name = "Lượt bán")]
    SellNumber  // sắp xếp theo số lượng đã bán,
}