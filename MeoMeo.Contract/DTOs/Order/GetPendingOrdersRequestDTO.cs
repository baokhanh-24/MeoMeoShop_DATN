using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Order
{
    public class GetPendingOrdersRequestDTO : BasePaging
    {
        public Guid? EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
