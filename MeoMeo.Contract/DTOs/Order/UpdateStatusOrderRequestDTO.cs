using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order;

public class UpdateStatusOrderRequestDTO
{
    public List<Guid> OrderIds { get; set; } 
    public EOrderStatus Status { get; set; }
}