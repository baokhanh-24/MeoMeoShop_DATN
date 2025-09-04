using MeoMeo.Contract.DTOs;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using System.Text.Json;

namespace MeoMeo.Shared.Services
{
    public class GhnService : IGhnService
    {
        private readonly IApiCaller _apiCaller;
        private readonly IConfiguration _configuration;
        private readonly string _ghnBaseUrl;
        private readonly string _ghnToken;
        private readonly int _ghnShopId;

        public GhnService(IApiCaller apiCaller, IConfiguration configuration)
        {
            _apiCaller = apiCaller;
            _configuration = configuration;
            _ghnBaseUrl = _configuration["Ghn:BaseUrl"] ?? "https://dev-online-gateway.ghn.vn/shiip/public-api";
            _ghnToken = _configuration["Ghn:Token"] ?? "";
            int.TryParse(_configuration["Ghn:ShopId"], out _ghnShopId);
        }

        public async Task<GhnCreateOrderResponseDTO?> CreateOrderAsync(GhnCreateOrderRequestDTO request)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_ghnBaseUrl}/v2/shipping-order/create");
                httpRequest.Headers.Add("token", _ghnToken);
                httpRequest.Headers.Add("shop_id", _ghnShopId.ToString());
                
                var jsonContent = JsonSerializer.Serialize(request);
                httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _apiCaller.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<GhnCreateOrderResponseDTO>(responseContent);
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error creating GHN order: {ex.Message}");
                return null;
            }
        }

        public async Task<decimal> CalculateShippingFeeAsync(int fromDistrictId, int toDistrictId, string toWardCode, int weight, int length, int width, int height, int serviceId, decimal insuranceValue = 0)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_ghnBaseUrl}/v2/shipping-order/fee");
                httpRequest.Headers.Add("token", _ghnToken);
                httpRequest.Headers.Add("shop_id", _ghnShopId.ToString());

                var requestBody = new
                {
                    service_id = serviceId,
                    insurance_value = (int)insuranceValue,
                    coupon = (string?)null,
                    from_district_id = fromDistrictId,
                    to_district_id = toDistrictId,
                    to_ward_code = toWardCode,
                    height = height,
                    length = length,
                    weight = weight,
                    width = width
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _apiCaller.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("data", out var data) && data.TryGetProperty("total", out var total))
                    {
                        return total.GetDecimal();
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating shipping fee: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<GhnLocationItem>> GetProvincesAsync()
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_ghnBaseUrl}/master-data/province");
                httpRequest.Headers.Add("token", _ghnToken);
                httpRequest.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

                var response = await _apiCaller.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                var provinces = new List<GhnLocationItem>();
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        foreach (var province in data.EnumerateArray())
                        {
                            provinces.Add(new GhnLocationItem
                            {
                                Id = province.GetProperty("ProvinceID").GetInt32(),
                                Name = province.GetProperty("ProvinceName").GetString() ?? "",
                                GhnProvinceId = province.GetProperty("ProvinceID").GetInt32()
                            });
                        }
                    }
                }

                return provinces;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting provinces: {ex.Message}");
                return new List<GhnLocationItem>();
            }
        }

        public async Task<List<GhnLocationItem>> GetDistrictsAsync(int provinceId)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_ghnBaseUrl}/master-data/district");
                httpRequest.Headers.Add("token", _ghnToken);

                var requestBody = new { province_id = provinceId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _apiCaller.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                var districts = new List<GhnLocationItem>();
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        foreach (var district in data.EnumerateArray())
                        {
                            districts.Add(new GhnLocationItem
                            {
                                Id = district.GetProperty("DistrictID").GetInt32(),
                                Name = district.GetProperty("DistrictName").GetString() ?? "",
                                GhnDistrictId = district.GetProperty("DistrictID").GetInt32()
                            });
                        }
                    }
                }

                return districts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting districts: {ex.Message}");
                return new List<GhnLocationItem>();
            }
        }

        public async Task<List<GhnLocationItem>> GetWardsAsync(int districtId)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_ghnBaseUrl}/master-data/ward");
                httpRequest.Headers.Add("token", _ghnToken);

                var requestBody = new { district_id = districtId };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _apiCaller.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                var wards = new List<GhnLocationItem>();
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        foreach (var ward in data.EnumerateArray())
                        {
                            wards.Add(new GhnLocationItem
                            {
                                Id = Convert.ToInt32(ward.GetProperty("WardCode").GetString() ?? "0"),
                                Name = ward.GetProperty("WardName").GetString() ?? "",
                                GhnWardCode = ward.GetProperty("WardCode").GetString()
                            });
                        }
                    }
                }

                return wards;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting wards: {ex.Message}");
                return new List<GhnLocationItem>();
            }
        }

        public async Task<List<GhnServiceItem>> GetAvailableServicesAsync(int fromDistrictId, int toDistrictId)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_ghnBaseUrl}/v2/shipping-order/available-services");
                httpRequest.Headers.Add("token", _ghnToken);

                var requestBody = new
                {
                    shop_id = _ghnShopId,
                    from_district = fromDistrictId,
                    to_district = toDistrictId
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                httpRequest.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await _apiCaller.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                var services = new List<GhnServiceItem>();
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("data", out var data))
                    {
                        foreach (var service in data.EnumerateArray())
                        {
                            services.Add(new GhnServiceItem
                            {
                                ServiceId = service.GetProperty("service_id").GetInt32(),
                                ServiceName = service.GetProperty("short_name").GetString() ?? "",
                                ServiceTypeId = service.GetProperty("service_type_id").GetInt32(),
                                ServiceTypeName = service.GetProperty("service_type_name").GetString() ?? ""
                            });
                        }
                    }
                }

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available services: {ex.Message}");
                return new List<GhnServiceItem>();
            }
        }
    }
}
