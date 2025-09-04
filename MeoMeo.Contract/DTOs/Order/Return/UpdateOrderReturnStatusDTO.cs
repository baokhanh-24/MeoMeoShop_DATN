using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order.Return;

public class UpdateOrderReturnStatusRequestDTO
{
    public Guid OrderReturnId { get; set; }
    public EOrderReturnStatus Status { get; set; }
    public string? Reason { get; set; }
}

public class UpdateOrderReturnStatusResponseDTO : BaseResponse
{
}


