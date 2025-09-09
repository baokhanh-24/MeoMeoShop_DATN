using System;
using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Contract.DTOs.Order.Return
{
    public class UpdatePayBackAmountDTO
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền hoàn trả phải lớn hơn 0")]
        public decimal PayBackAmount { get; set; }

        public DateTime? PayBackDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Note { get; set; }
    }
}
