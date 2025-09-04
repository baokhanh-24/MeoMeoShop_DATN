using MeoMeo.Contract.DTOs;

namespace MeoMeo.Shared.IServices
{
    public interface IGhnService
    {
        Task<GhnCreateOrderResponseDTO?> CreateOrderAsync(GhnCreateOrderRequestDTO request);
        Task<decimal> CalculateShippingFeeAsync(int fromDistrictId, int toDistrictId, string toWardCode, int weight, int length, int width, int height, int serviceId, decimal insuranceValue = 0);
        Task<List<GhnLocationItem>> GetProvincesAsync();
        Task<List<GhnLocationItem>> GetDistrictsAsync(int provinceId);
        Task<List<GhnLocationItem>> GetWardsAsync(int districtId);
        Task<List<GhnServiceItem>> GetAvailableServicesAsync(int fromDistrictId, int toDistrictId);
    }

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
        public string ServiceTypeName { get; set; } = "";
    }
}
