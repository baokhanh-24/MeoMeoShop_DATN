using MeoMeo.Contract.Commons;

namespace MeoMeo.Contract.DTOs.Auth;

public class AuthenResponse:BaseResponse
{
    public string AccessToken { get; set; }
    public int ExpriesIn { get; set; }
    public string TokenType { get; set; }
    public string RefreshToken { get; set; }
}