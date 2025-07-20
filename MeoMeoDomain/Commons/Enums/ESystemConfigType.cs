using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Domain.Commons.Enums
{
    public enum ESystemConfigType
    {
        [Display(Name = "Địa chỉ cửa hàng")]
        AddressShop,
        [Display(Name = "Link website")]
        WebsiteLink,
        [Display(Name = "Hotline cửa hàng")]
        StoreHotline,
        [Display(Name = "Tên cửa hàng")]
        StoreName,
        [Display(Name = "Copyright")]
        Copyright,
        [Display(Name = "Giờ làm việc")]
        WorkingTime
    }
}
