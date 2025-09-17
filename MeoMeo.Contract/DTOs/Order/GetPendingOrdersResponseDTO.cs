using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons;

namespace MeoMeo.Contract.DTOs.Order
{
    public class GetPendingOrdersResponseDTO : PagingExtensions.PagedResult<PendingOrderDTO>
    {
        public int TotalPendingCount { get; set; }
        public int TotalDraftCount { get; set; }
        public decimal TotalPendingAmount { get; set; }

        // Override TotalCount to use TotalPendingCount
        public int TotalCount => TotalPendingCount;
    }
}
