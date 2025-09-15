using MeoMeo.Contract.Commons;
using MeoMeo.Contract.DTOs;
using MeoMeo.Contract.DTOs.Auth;
using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.Shared.IServices;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using MeoMeo.Application.IServices;
using MeoMeo.Domain.Commons;
using Microsoft.EntityFrameworkCore;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Application.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IResetPasswordHistoryRepository _resetPasswordHistoryRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<PasswordResetService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PasswordResetService(
            IUserRepository userRepository,
            IResetPasswordHistoryRepository resetPasswordHistoryRepository,
            IEmailService emailService,
            ILogger<PasswordResetService> logger,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _resetPasswordHistoryRepository = resetPasswordHistoryRepository;
            _emailService = emailService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> RequestPasswordResetAsync(string email)
        {
            try
            {
                var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Email không tồn tại trong hệ thống"
                    };
                }

                // Tạo reset token
                var resetToken = GenerateResetToken();
                var expiresAt = DateTime.UtcNow.AddHours(24); // Token hết hạn sau 24 giờ

                // Lưu reset password history
                var resetHistory = new ResetPasswordHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Code = resetToken,
                    ExpriedDate = expiresAt,
                    CreationTime = DateTime.UtcNow,
                    Status = EResetPasswordHistoryStatus.PendingApproval
                };

                await _resetPasswordHistoryRepository.AddAsync(resetHistory);
                await _unitOfWork.SaveChangesAsync();

                // Gửi email reset password
                var emailSent = await _emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    resetToken,
                    user.UserName
                );

                if (!emailSent)
                {
                    _logger.LogWarning($"Failed to send password reset email to {email}");
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không thể gửi email reset password. Vui lòng thử lại sau."
                    };
                }

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Email reset password đã được gửi đến địa chỉ email của bạn."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error requesting password reset for email {email}");
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi xử lý yêu cầu reset password."
                };
            }
        }

        public async Task<BaseResponse> ResetPasswordAsync(string resetToken, string newPassword)
        {
            try
            {
                var resetHistory = await _resetPasswordHistoryRepository.Query()
                    .FirstOrDefaultAsync(r => r.Code == resetToken);
                if (resetHistory == null)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Token reset password không hợp lệ"
                    };
                }

                if (resetHistory.Status == EResetPasswordHistoryStatus.Approved)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Token reset password đã được sử dụng"
                    };
                }

                if (resetHistory.ExpriedDate < DateTime.UtcNow)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Token reset password đã hết hạn"
                    };
                }

                // Cập nhật mật khẩu user
                var user = await _userRepository.GetUserByIdAsync(resetHistory.UserId);
                if (user == null)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Không tìm thấy người dùng"
                    };
                }

                user.PasswordHash = ComputeSha256Hash(newPassword);
                await _userRepository.UpdateUserAsync(user);

                // Đánh dấu token đã sử dụng
                resetHistory.Status = EResetPasswordHistoryStatus.Approved;
                resetHistory.SuccessDate = DateTime.UtcNow;
                await _resetPasswordHistoryRepository.UpdateAsync(resetHistory);

                await _unitOfWork.SaveChangesAsync();

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Đặt lại mật khẩu thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password with token {resetToken}");
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi đặt lại mật khẩu"
                };
            }
        }

        public async Task<BaseResponse> ValidateResetTokenAsync(string resetToken)
        {
            try
            {
                var resetHistory = await _resetPasswordHistoryRepository.Query()
                    .FirstOrDefaultAsync(r => r.Code == resetToken);
                if (resetHistory == null)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Token reset password không hợp lệ"
                    };
                }

                if (resetHistory.Status == EResetPasswordHistoryStatus.Approved)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Token reset password đã được sử dụng"
                    };
                }

                if (resetHistory.ExpriedDate < DateTime.UtcNow)
                {
                    return new BaseResponse
                    {
                        ResponseStatus = BaseStatus.Error,
                        Message = "Token reset password đã hết hạn"
                    };
                }

                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Success,
                    Message = "Token hợp lệ"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating reset token {resetToken}");
                return new BaseResponse
                {
                    ResponseStatus = BaseStatus.Error,
                    Message = "Có lỗi xảy ra khi xác thực token"
                };
            }
        }

        private string GenerateResetToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string ComputeSha256Hash(string rawData)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }
    }
}
