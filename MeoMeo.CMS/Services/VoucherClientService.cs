using MeoMeo.CMS.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Domain.Commons;
using MeoMeo.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MeoMeo.CMS.Services
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
                var result = await _httpClient.PostAsync<CreateOrUpdateVoucherDTO, CreateOrUpdateVoucherResponseDTO>(url, voucher);
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
                var response = await _httpClient.GetAsync<PagingExtensions.PagedResult<VoucherDTO>>(url);
                return response ?? new PagingExtensions.PagedResult<VoucherDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra khi lấy danh sách voucher: {Message}", ex.Message);
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
                var result = await _httpClient.PutAsync<CreateOrUpdateVoucherDTO, CreateOrUpdateVoucherResponseDTO>(url, voucher);
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
    }
}
