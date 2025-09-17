using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Contract.DTOs.Order
{
    public class CancelOrderWithReasonRequestDTO
    {
        [Required(ErrorMessage = "Lý do hủy đơn hàng là bắt buộc")]
        [StringLength(500, ErrorMessage = "Lý do hủy đơn hàng không được vượt quá 500 ký tự")]
        public string Reason { get; set; } = string.Empty;
    }
}
