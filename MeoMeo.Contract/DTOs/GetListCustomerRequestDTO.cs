using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs;

public class GetListCustomerRequestDTO:BasePaging
{
    public string? FullNameFilter { get; set; }
    public string? PhoneNumberFilter { get; set; }
    public ECustomerStatus? StatusFilter { get; set; }
    public string? CodeFilter { get; set; }
    public DateOnly? DateOfBirthFilter { get; set; }
    public string? TaxCodeFilter { get; set; }
    public string? AddressFilter { get; set; }
}