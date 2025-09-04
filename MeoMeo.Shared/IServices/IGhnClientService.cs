using MeoMeo.Contract.DTOs;
using MeoMeo.Shared.DTOs;

namespace MeoMeo.Shared.IServices
{
    public interface IGhnClientService
    {
        Task<GhnCreateOrderResponseDTO?> CreateOrderAsync(GhnCreateOrderRequestDTO request);
        Task<decimal> CalculateShippingFeeAsync(int fromDistrictId, int toDistrictId, string toWardCode, int weight, int length, int width, int height, int serviceId, decimal insuranceValue = 0);
        Task<List<GhnLocationItem>> GetProvincesAsync();
        Task<List<GhnLocationItem>> GetDistrictsAsync(int provinceId);
        Task<List<GhnLocationItem>> GetWardsAsync(int districtId);
        Task<List<GhnServiceItem>> GetAvailableServicesAsync(int fromDistrictId, int toDistrictId);
    }


}
