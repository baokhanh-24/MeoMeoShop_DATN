using MeoMeo.Contract.Commons;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Order.Return;

public class CreateOrderReturnRequestDTO
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ERefundMethod RefundMethod { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactName { get; set; }
    public List<CreateOrderReturnItemDTO> Items { get; set; } = new();
    public List<CreateOrderReturnFileDTO> Files { get; set; } = new();
}

public class CreateOrderReturnItemDTO
{
    public Guid OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
}

public class CreateOrderReturnFileDTO
{
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public class CreateOrderReturnResponseDTO : BaseResponse
{
    public Guid? OrderReturnId { get; set; }
    public string? Code { get; set; }
}


