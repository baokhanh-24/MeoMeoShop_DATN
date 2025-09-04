using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Order;

public class CreateOrderResultDTO : BaseResponse
{
    public Guid OrderId { get; set; }
    public string Code { get; set; }
    public string DeliveryAddress { get; set; }
    public string Note { get; set; }
}


