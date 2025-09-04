namespace MeoMeo.Shared.DTOs;

public class GhnLocationItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int GhnProvinceId { get; set; }
    public int GhnDistrictId { get; set; }
    public string? GhnWardCode { get; set; }
}

public class GhnServiceItem
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = "";
    public int ServiceTypeId { get; set; }
}