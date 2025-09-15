using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs;

public class EmployeeDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBird { get; set; }
    public string Address { get; set; }
    public int Status { get; set; }
    public EEmployeesStatus StatusEnum
    {
        get => (EEmployeesStatus)Status;
        set => Status = (int)value;
    }
}
