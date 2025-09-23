using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Order.Return;
using MeoMeo.Domain.Commons;
using MeoMeo.Shared.IServices;
using MeoMeo.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Linq;

namespace MeoMeo.Shared.Services
{
    public class OrderReturnClientService : IOrderReturnClientService
    {
        private readonly IApiCaller _apiCaller;

        public OrderReturnClientService(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
        }

        public async Task<BaseResponse> CreatePartialOrderReturnAsync(CreatePartialOrderReturnDTO request)
        {
            try
            {
                var url = "api/OrderReturn/create-partial-return";
                var result = await _apiCaller.PostAsync<CreatePartialOrderReturnDTO, BaseResponse>(url, request);
                return result ?? new BaseResponse
                {
                    Message = "Lỗi khi tạo yêu cầu hoàn trả",
                    ResponseStatus = BaseStatus.Error
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> CreatePartialOrderReturnAsync(CreatePartialOrderReturnDTO request, List<ReturnFileUpload> files)
        {
            try
            {
                var url = "api/OrderReturn/create-partial-return";

                using var formData = new MultipartFormDataContent();

                // Add basic fields
                formData.Add(new StringContent(request.OrderId.ToString()), "OrderId");
                formData.Add(new StringContent(request.Reason), "Reason");
                formData.Add(new StringContent(((int)request.RefundMethod).ToString()), "RefundMethod");

                // Add bank info if provided
                if (!string.IsNullOrEmpty(request.BankName))
                    formData.Add(new StringContent(request.BankName), "BankName");
                if (!string.IsNullOrEmpty(request.BankAccountName))
                    formData.Add(new StringContent(request.BankAccountName), "BankAccountName");
                if (!string.IsNullOrEmpty(request.BankAccountNumber))
                    formData.Add(new StringContent(request.BankAccountNumber), "BankAccountNumber");

                // Add contact info if provided
                if (!string.IsNullOrEmpty(request.ContactName))
                    formData.Add(new StringContent(request.ContactName), "ContactName");
                if (!string.IsNullOrEmpty(request.ContactPhone))
                    formData.Add(new StringContent(request.ContactPhone), "ContactPhone");

                // Add items
                for (int i = 0; i < request.Items.Count; i++)
                {
                    var item = request.Items[i];
                    formData.Add(new StringContent(item.OrderDetailId.ToString()), $"Items[{i}].OrderDetailId");
                    formData.Add(new StringContent(item.Quantity.ToString()), $"Items[{i}].Quantity");
                    if (!string.IsNullOrEmpty(item.Reason))
                        formData.Add(new StringContent(item.Reason), $"Items[{i}].Reason");
                }

                // Add files
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var fileBytes = Convert.FromBase64String(file.Base64Data);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(file.ContentType);
                    formData.Add(fileContent, $"FileUploads[{i}].UploadFile", file.FileName);
                    formData.Add(new StringContent(file.FileName), $"FileUploads[{i}].FileName");
                    formData.Add(new StringContent(file.ContentType), $"FileUploads[{i}].ContentType");
                }

                var result = await _apiCaller.PostFormAsync<BaseResponse>(url, formData);
                return result ?? new BaseResponse
                {
                    Message = "Lỗi khi tạo yêu cầu hoàn trả",
                    ResponseStatus = BaseStatus.Error
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<PagingExtensions.PagedResult<OrderReturnListDTO>?> GetMyOrderReturnsAsync(GetOrderReturnRequestDTO request)
        {
            try
            {
                var queryString = BuildQueryString(request);
                var url = $"api/OrderReturn/my-returns{queryString}";
                var result = await _apiCaller.GetAsync<PagingExtensions.PagedResult<OrderReturnListDTO>>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting my order returns: {ex.Message}");
                return null;
            }
        }

        public async Task<OrderReturnDetailDTO?> GetOrderReturnByIdAsync(Guid id)
        {
            try
            {
                var url = $"api/OrderReturn/{id}";
                var result = await _apiCaller.GetAsync<OrderReturnDetailDTO>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order return detail: {ex.Message}");
                return null;
            }
        }

        public async Task<BaseResponse> CancelOrderReturnAsync(Guid orderReturnId)
        {
            try
            {
                var url = $"api/OrderReturn/{orderReturnId}/cancel";
                var result = await _apiCaller.PostAsync<object, BaseResponse>(url, new { });
                return result ?? new BaseResponse
                {
                    Message = "Lỗi khi hủy yêu cầu hoàn trả",
                    ResponseStatus = BaseStatus.Error
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<List<OrderReturnItemDetailDTO>?> GetAvailableItemsForReturnAsync(Guid orderId)
        {
            try
            {
                var url = $"api/OrderReturn/order/{orderId}/available-items";
                var result = await _apiCaller.GetAsync<List<OrderReturnItemDetailDTO>>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting available items for return: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CanOrderBeReturnedAsync(Guid orderId)
        {
            try
            {
                var url = $"api/OrderReturn/order/{orderId}/can-return";
                var result = await _apiCaller.GetAsync<bool>(url);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if order can be returned: {ex.Message}");
                return false;
            }
        }

        public async Task<List<OrderReturnListDTO>?> GetByOrderIdAsync(Guid orderId)
        {
            try
            {
                var request = new GetOrderReturnRequestDTO
                {
                    PageIndex = 1,
                    PageSize = 100 // Get all returns for this order
                };

                var result = await GetMyOrderReturnsAsync(request);
                return result?.Items?.Where(r => r.OrderId == orderId).ToList() ?? new List<OrderReturnListDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order returns by order id: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool CanReturn, string Message, List<string> ReturnableProducts, List<string> NonReturnableProducts)?> GetOrderReturnInfoAsync(Guid orderId)
        {
            try
            {
                var url = $"api/OrderReturn/order/{orderId}/return-info";
                var result = await _apiCaller.GetAsync<dynamic>(url);

                if (result != null)
                {
                    return (
                        (bool)result.canReturn,
                        (string)result.message,
                        ((IEnumerable<object>)result.returnableProducts).Cast<string>().ToList(),
                        ((IEnumerable<object>)result.nonReturnableProducts).Cast<string>().ToList()
                    );
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order return info: {ex.Message}");
                return null;
            }
        }

        private string BuildQueryString(GetOrderReturnRequestDTO request)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.CodeFilter))
                queryParams.Add($"CodeFilter={Uri.EscapeDataString(request.CodeFilter)}");

            if (!string.IsNullOrEmpty(request.OrderCodeFilter))
                queryParams.Add($"OrderCodeFilter={Uri.EscapeDataString(request.OrderCodeFilter)}");

            if (request.StatusFilter.HasValue)
                queryParams.Add($"StatusFilter={request.StatusFilter.Value}");

            if (request.RefundMethodFilter.HasValue)
                queryParams.Add($"RefundMethodFilter={request.RefundMethodFilter.Value}");

            if (request.FromDateFilter.HasValue)
                queryParams.Add($"FromDateFilter={request.FromDateFilter.Value:yyyy-MM-dd}");

            if (request.ToDateFilter.HasValue)
                queryParams.Add($"ToDateFilter={request.ToDateFilter.Value:yyyy-MM-dd}");

            queryParams.Add($"PageIndex={request.PageIndex}");
            queryParams.Add($"PageSize={request.PageSize}");

            return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        }

        public async Task<BaseResponse> UpdateOrderReturnStatusAsync(Guid orderReturnId, UpdateOrderReturnStatusRequestDTO request)
        {
            try
            {
                // Set OrderReturnId in request body
                request.OrderReturnId = orderReturnId;

                var url = "api/OrderReturn/update-status";
                var result = await _apiCaller.PutAsync<UpdateOrderReturnStatusRequestDTO, BaseResponse>(url, request);
                return result ?? new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Không thể cập nhật trạng thái hoàn hàng"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order return status: {ex.Message}");
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = ex.Message
                };
            }
        }
    }
}