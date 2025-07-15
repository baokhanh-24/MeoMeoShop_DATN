using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order;

public class GetListOrderHistoryResponseDTO : BaseResponse
{
    public List<OrderHistoryDTO> Items { get; set; }
}
public class OrderHistoryDTO:IHasHistoryInfo
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public DateTime CreationTime { get; set; }
    public string Content { get; set; }
    public string Actor { get; set; }
    public EHistoryType Type { get; set; }
}