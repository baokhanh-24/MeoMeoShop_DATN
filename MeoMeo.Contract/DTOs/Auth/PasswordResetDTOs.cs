using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Auth
{
    public class RequestPasswordResetDTO
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDTO
    {
        public string ResetToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ValidateResetTokenDTO
    {
        public string ResetToken { get; set; } = string.Empty;
    }
}
