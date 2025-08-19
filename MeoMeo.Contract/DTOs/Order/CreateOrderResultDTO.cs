using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Order;

public class CreateOrderResultDTO : BaseResponse
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}


