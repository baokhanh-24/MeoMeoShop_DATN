using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Order;

public class GetListOrderResponseDTO:BaseResponse
{
    public int TotalAll { get; set; }
    public int Pending { get; set; }
    public int Confirmed { get; set; }
    public int InTransit { get; set; }
    public int Canceled { get; set; }
    public int Completed { get; set; }
}