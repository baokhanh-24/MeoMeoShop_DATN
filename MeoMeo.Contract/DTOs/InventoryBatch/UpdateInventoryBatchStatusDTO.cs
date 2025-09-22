using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.InventoryBatch;

public class UpdateInventoryBatchStatusDTO
{
    public Guid Id { get; set; }
    public EInventoryBatchStatus Status { get; set; }
    public string? Reason { get; set; }
}
