using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Contract.DTOs
{
    public class CreateQuickCustomerDTO
    {
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên khách hàng không được vượt quá 255 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string? Address { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Note { get; set; }
    }
}
