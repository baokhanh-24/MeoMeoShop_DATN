using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs;

public class CustomerDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreationTime { get; set; }
    public string? TaxCode { get; set; }
    public string? Address { get; set; }
    public ECustomerStatus Status { get; set; }
}