using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs;

public class CreateOrUpdateCardResponse:BaseResponse
{
    public Guid CustomerId { get; set; }
    public decimal TotalPrice { get; set; }
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public DateTime? LastModificationTime { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}