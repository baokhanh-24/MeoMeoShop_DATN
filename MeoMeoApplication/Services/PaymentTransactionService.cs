using AutoMapper;
using MeoMeo.Application.IServices;
using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs.Payment;
using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Commons.Enums;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeoMeo.Application.Services
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentTransactionService> _logger;

        public PaymentTransactionService(
            IPaymentTransactionRepository paymentTransactionRepository,
            IOrderRepository orderRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<PaymentTransactionService> logger)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreatePaymentTransactionResponseDTO> CreatePaymentTransactionAsync(CreatePaymentTransactionDTO request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate order exists
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return new CreatePaymentTransactionResponseDTO
                    {
                        Message = "Không tìm thấy đơn hàng",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Generate transaction code
                var transactionCode = GenerateTransactionCode();

                var transaction = new PaymentTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    CustomerId = request.CustomerId,
                    TransactionCode = transactionCode,
                    Amount = request.Amount,
                    VnpOrderInfo = request.Description,
                    Status = EPaymentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    IpAddress = request.IpAddress,
                    PaymentMethod = request.PaymentMethod,
                    Currency = "VND"
                };

                await _paymentTransactionRepository.AddAsync(transaction);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Created payment transaction {TransactionCode} for order {OrderId}",
                    transactionCode, request.OrderId);

                return new CreatePaymentTransactionResponseDTO
                {
                    TransactionCode =  transactionCode,
                    Message = "Tạo giao dịch thanh toán thành công",
                    ResponseStatus = BaseStatus.Success,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating payment transaction for order {OrderId}", request.OrderId);

                return new CreatePaymentTransactionResponseDTO
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> ProcessVnpayCallbackAsync(VnpayCallbackDTO callback, string rawData)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Find transaction by VnpTxnRef (transaction code)
                var transaction = await _paymentTransactionRepository.Query()
                    .Include(t => t.Order)
                    .FirstOrDefaultAsync(t => t.TransactionCode == callback.vnp_TxnRef);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction not found for VnpTxnRef: {TxnRef}", callback.vnp_TxnRef);
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy giao dịch",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Update transaction with VNPay response
                transaction.VnpTransactionNo = callback.vnp_TransactionNo;
                transaction.VnpTxnRef = callback.vnp_TxnRef;
                transaction.VnpBankCode = callback.vnp_BankCode;
                transaction.VnpBankTranNo = callback.vnp_BankTranNo;
                transaction.VnpCardType = callback.vnp_CardType;
                transaction.VnpPayDate = callback.vnp_PayDate;
                transaction.VnpResponseCode = callback.vnp_ResponseCode;
                transaction.VnpTransactionStatus = callback.vnp_TransactionStatus;
                transaction.VnpSecureHash = callback.vnp_SecureHash;
                transaction.RawResponseData = rawData;
                transaction.ProcessedAt = DateTime.Now;

                // Determine transaction status
                if (callback.vnp_ResponseCode == "00" && callback.vnp_TransactionStatus == "00")
                {
                    transaction.Status = EPaymentStatus.Success;
                    transaction.CompletedAt = DateTime.Now;
                    transaction.ResponseMessage = "Thanh toán thành công";

                    // Update order status if needed
                    if (transaction.Order.Status == EOrderStatus.Pending)
                    {
                        transaction.Order.Status = EOrderStatus.Confirmed;
                        await _orderRepository.UpdateAsync(transaction.Order);
                    }

                    _logger.LogInformation("Payment successful for transaction {TransactionCode}", transaction.TransactionCode);
                }
                else
                {
                    transaction.Status = EPaymentStatus.Failed;
                    transaction.ErrorCode = callback.vnp_ResponseCode;
                    transaction.ErrorMessage = GetVnpayErrorMessage(callback.vnp_ResponseCode);
                    transaction.ResponseMessage = transaction.ErrorMessage;

                    _logger.LogWarning("Payment failed for transaction {TransactionCode}, Error: {ErrorCode}",
                        transaction.TransactionCode, callback.vnp_ResponseCode);
                }

                transaction.IsValidated = true; // Should validate hash in production
                await _paymentTransactionRepository.UpdateAsync(transaction);
                await _unitOfWork.CommitAsync();

                return new BaseResponse
                {
                    Message = transaction.ResponseMessage,
                    ResponseStatus = transaction.Status == EPaymentStatus.Success ? BaseStatus.Success : BaseStatus.Error
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error processing VNPay callback for TxnRef: {TxnRef}", callback.vnp_TxnRef);

                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<PaymentTransactionDTO?> GetByTransactionCodeAsync(string transactionCode)
        {
            try
            {
                var transaction = await _paymentTransactionRepository.Query()
                    .Include(t => t.Order)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);

                if (transaction == null) return null;

                var dto = _mapper.Map<PaymentTransactionDTO>(transaction);
                dto.StatusDisplayName = GetStatusDisplayName(transaction.Status);
                dto.OrderCode = transaction.Order?.Code ?? "";
                dto.CustomerName = transaction.Customer?.Name ?? "";

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction by code: {TransactionCode}", transactionCode);
                return null;
            }
        }

        public async Task<PaymentTransactionDTO?> GetByOrderIdAsync(Guid orderId)
        {
            try
            {
                var transaction = await _paymentTransactionRepository.Query()
                    .Include(t => t.Order)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.OrderId == orderId);

                if (transaction == null) return null;

                var dto = _mapper.Map<PaymentTransactionDTO>(transaction);
                dto.StatusDisplayName = GetStatusDisplayName(transaction.Status);
                dto.OrderCode = transaction.Order?.Code ?? "";
                dto.CustomerName = transaction.Customer?.Name ?? "";

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction by order id: {OrderId}", orderId);
                return null;
            }
        }

        public async Task<List<PaymentTransactionDTO>> GetByCustomerIdAsync(Guid customerId)
        {
            try
            {
                var transactions = await _paymentTransactionRepository.Query()
                    .Include(t => t.Order)
                    .Include(t => t.Customer)
                    .Where(t => t.CustomerId == customerId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return transactions.Select(t =>
                {
                    var dto = _mapper.Map<PaymentTransactionDTO>(t);
                    dto.StatusDisplayName = GetStatusDisplayName(t.Status);
                    dto.OrderCode = t.Order?.Code ?? "";
                    dto.CustomerName = t.Customer?.Name ?? "";
                    return dto;
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions by customer id: {CustomerId}", customerId);
                return new List<PaymentTransactionDTO>();
            }
        }

        public async Task<BaseResponse> RetryFailedTransactionAsync(Guid transactionId)
        {
            try
            {
                var transaction = await _paymentTransactionRepository.GetByIdAsync(transactionId);
                if (transaction == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy giao dịch",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                if (transaction.Status != EPaymentStatus.Failed)
                {
                    return new BaseResponse
                    {
                        Message = "Chỉ có thể retry giao dịch thất bại",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                transaction.Status = EPaymentStatus.Pending;
                transaction.RetryCount++;
                transaction.ErrorCode = null;
                transaction.ErrorMessage = null;

                await _paymentTransactionRepository.UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    Message = "Đã reset giao dịch để retry",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying transaction: {TransactionId}", transactionId);
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<BaseResponse> RefundTransactionAsync(Guid transactionId, decimal refundAmount, string reason)
        {
            try
            {
                var transaction = await _paymentTransactionRepository.GetByIdAsync(transactionId);
                if (transaction == null)
                {
                    return new BaseResponse
                    {
                        Message = "Không tìm thấy giao dịch",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                if (transaction.Status != EPaymentStatus.Success)
                {
                    return new BaseResponse
                    {
                        Message = "Chỉ có thể hoàn tiền giao dịch thành công",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                if (refundAmount > transaction.Amount)
                {
                    return new BaseResponse
                    {
                        Message = "Số tiền hoàn không được vượt quá số tiền giao dịch",
                        ResponseStatus = BaseStatus.Error
                    };
                }

                // Create refund record (you might want a separate RefundTransaction table)
                transaction.Status = refundAmount == transaction.Amount
                    ? EPaymentStatus.Refunded
                    : EPaymentStatus.PartiallyRefunded;

                transaction.ResponseMessage = $"Đã hoàn tiền: {refundAmount:N0} VNĐ. Lý do: {reason}";

                await _paymentTransactionRepository.UpdateAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Refunded transaction {TransactionCode}, Amount: {RefundAmount}",
                    transaction.TransactionCode, refundAmount);

                return new BaseResponse
                {
                    Message = "Hoàn tiền thành công",
                    ResponseStatus = BaseStatus.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refunding transaction: {TransactionId}", transactionId);
                return new BaseResponse
                {
                    Message = ex.Message,
                    ResponseStatus = BaseStatus.Error
                };
            }
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _paymentTransactionRepository.Query()
                    .Where(t => t.Status == EPaymentStatus.Success);

                if (fromDate.HasValue)
                    query = query.Where(t => t.CompletedAt >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.CompletedAt <= toDate.Value);

                return await query.SumAsync(t => t.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total revenue");
                return 0;
            }
        }

        public async Task<int> GetSuccessfulTransactionCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _paymentTransactionRepository.Query()
                    .Where(t => t.Status == EPaymentStatus.Success);

                if (fromDate.HasValue)
                    query = query.Where(t => t.CompletedAt >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.CompletedAt <= toDate.Value);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting successful transactions");
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetPaymentMethodStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _paymentTransactionRepository.Query()
                    .Where(t => t.Status == EPaymentStatus.Success);

                if (fromDate.HasValue)
                    query = query.Where(t => t.CompletedAt >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(t => t.CompletedAt <= toDate.Value);

                return await query
                    .GroupBy(t => t.VnpBankCode)
                    .Select(g => new { BankCode = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.BankCode, x => x.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment method stats");
                return new Dictionary<string, int>();
            }
        }

        private string GenerateTransactionCode()
        {
            return $"TXN{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }

        private string GetStatusDisplayName(EPaymentStatus status)
        {
            return status switch
            {
                EPaymentStatus.Pending => "Chờ thanh toán",
                EPaymentStatus.Processing => "Đang xử lý",
                EPaymentStatus.Success => "Thành công",
                EPaymentStatus.Failed => "Thất bại",
                EPaymentStatus.Cancelled => "Đã hủy",
                EPaymentStatus.Refunded => "Đã hoàn tiền",
                EPaymentStatus.PartiallyRefunded => "Hoàn tiền một phần",
                EPaymentStatus.Expired => "Hết hạn",
                EPaymentStatus.SystemError => "Lỗi hệ thống",
                _ => status.ToString()
            };
        }

        private string GetVnpayErrorMessage(string errorCode)
        {
            return errorCode switch
            {
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
                "75" => "Ngân hàng thanh toán đang bảo trì.",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
                "99" => "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)",
                _ => $"Lỗi không xác định: {errorCode}"
            };
        }
    }
}
