namespace MeoMeo.Domain.Commons.Enums;

public enum EProductDetailSortField
{
    Name, // sắp xếp theo tên
    Price,  // sắp xếp theo giá
    Discount,  // sắp xếp % giảm
    CreationTime,  // hàng mới về
    StockQuantity , // sắp xếp theo số lượng tồn,
    ViewNumber,  // sắp xếp theo số lượt xem,
    SellNumber  // sắp xếp theo số lượng đã bán,
}