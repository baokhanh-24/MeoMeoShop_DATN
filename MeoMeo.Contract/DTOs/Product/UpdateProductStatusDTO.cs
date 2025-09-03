using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Contract.DTOs.Product;

public class UpdateProductStatusDTO
{
    public Guid Id { get; set; }
    public EProductStatus Status { get; set; }
}