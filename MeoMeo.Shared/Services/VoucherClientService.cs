
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;

namespace MeoMeo.Shared.Services
{
    public class VoucherClientService : IVoucherClientService
    {
        private readonly IApiCaller _httpClient;
        private readonly ILogger<VoucherClientService> _logger;

        public VoucherClientService(IApiCaller httpClient, ILogger<VoucherClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CreateOrUpdateVoucherResponseDTO> CreateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            try
            {
                var url = "/api/Vouchers/create-voucher-async";
                var result =
                    await _httpClient.PostAsync<CreateOrUpdateVoucherDTO, CreateOrUpdateVoucherResponseDTO>(url,
                        voucher);
                return result ?? new CreateOrUpdateVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo voucher: {Message}", ex.Message);
                return new CreateOrUpdateVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi tạo voucher"
                };
            }
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            try
            {
                var url = $"/api/Vouchers/delete-voucher-async/{id}";
                var result = await _httpClient.DeleteAsync(url);
                if (!result)
                {
                    _logger.LogWarning("Không thể xóa voucher với Id {Id}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi xóa voucher với Id {Id}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<PagingExtensions.PagedResult<VoucherDTO>> GetAllVoucherAsync(GetListVoucherRequestDTO request)
        {
            try
            {
                var query = BuildQuery.ToQueryString(request);
                var url = $"/api/Vouchers/get-all-voucher-async?{query}";
                Console.WriteLine($"Calling Voucher API: {url}");

                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<VoucherDTO>>(url);
                Console.WriteLine($"Voucher API response: {response?.Items?.Count ?? 0} items");

                return response ?? new PagingExtensions.PagedResult<VoucherDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách voucher: {Message}", ex.Message);
                Console.WriteLine($"Voucher API Error: {ex.Message}");
                return new PagingExtensions.PagedResult<VoucherDTO>();
            }
        }

        public async Task<CreateOrUpdateVoucherResponseDTO> GetVoucherByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/Vouchers/find-voucher-by-id-async/{id}";
                var response = await _httpClient.GetAsync<CreateOrUpdateVoucherResponseDTO>(url);
                return response ?? new CreateOrUpdateVoucherResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy voucher Id {Id}: {Message}", id, ex.Message);
                return new CreateOrUpdateVoucherResponseDTO();
            }
        }

        public async Task<CreateOrUpdateVoucherResponseDTO> UpdateVoucherAsync(CreateOrUpdateVoucherDTO voucher)
        {
            try
            {
                var url = $"/api/Vouchers/update-voucher-async/{voucher.Id}";
                var result =
                    await _httpClient
                        .PutAsync<CreateOrUpdateVoucherDTO, CreateOrUpdateVoucherResponseDTO>(url, voucher);
                return result ?? new CreateOrUpdateVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi cập nhật voucher: {Message}", ex.Message);
                return new CreateOrUpdateVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi cập nhật voucher"
                };
            }
        }

        public async Task<CheckVoucherResponseDTO> CheckVoucherAsync(CheckVoucherRequestDTO request)
        {
            try
            {
                var url = "/api/Vouchers/check-voucher-async";
                var result = await _httpClient.PostAsync<CheckVoucherRequestDTO, CheckVoucherResponseDTO>(url, request);
                return result ?? new CheckVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không có dữ liệu trả về"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi kiểm tra voucher: {Message}", ex.Message);
                return new CheckVoucherResponseDTO
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi kiểm tra voucher"
                };
            }
        }

        public async Task<List<AvailableVoucherDTO>> GetAvailableVouchersAsync(GetAvailableVouchersRequestDTO request)
        {
            try
            {
                var url = "/api/Vouchers/get-available-vouchers-async";
                var result =
                    await _httpClient
                        .PostAsync<GetAvailableVouchersRequestDTO, List<AvailableVoucherDTO>>(url, request);
                return result ?? new List<AvailableVoucherDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách voucher khả dụng: {Message}", ex.Message);
                return new List<AvailableVoucherDTO>();
            }
        }

        public async Task<string> GenerateUniqueVoucherCodeAsync()
        {
            try
            {
                var url = "/api/Vouchers/generate-unique-voucher-code-async";
                var result = await _httpClient.GetAsync<string>(url);
                return result ?? $"VOUCHER-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi tạo mã voucher duy nhất: {Message}", ex.Message);
                return $"VOUCHER-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            }
        }
    }
}
