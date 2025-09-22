using System;

namespace MeoMeo.Contract.DTOs
{
    public class GetMyAvailableVouchersRequestDTO
    {
        public decimal OrderAmount { get; set; }
        // Không cần CustomerId vì sẽ lấy từ JWT token của user đang đăng nhập
    }
}
