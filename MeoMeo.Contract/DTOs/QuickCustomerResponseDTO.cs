using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs
{
    public class QuickCustomerResponseDTO : BaseResponse
    {
        public Guid? CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
