using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums;

public enum ShippingMethod
{
    [Display(Name = "Tiêu chuẩn", Description = "3-5 ngày nhận hàng")]
    /// <summary>
    /// 1 - Giao hàng tiêu chuẩn (3-5 ngày làm việc)
    /// </summary>
    Standard = 53321,
    [Display(Name = "Nhanh", Description = "1-2 ngày nhận hàng")]
    /// <summary>
    /// 2 - Giao hàng nhanh ( ngày làm việc)
    /// </summary>
    Express = 53319,  
    [Display(Name = "Nhận tại cửa hàng", Description = "Nhận tại cửa hàng")]
    /// <summary>
    /// 3 - Nhận tại cửa hàng (Khách hàng đến cửa hàng lấy)
    /// </summary>
    Pickup = 0,
    
}