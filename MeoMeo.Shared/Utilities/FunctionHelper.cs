using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using MeoMeo.Domain.Commons.Enums;

namespace MeoMeo.Shared.Utilities;

public static class FunctionHelper
{
    public static string ComputerSha256Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        // ComputeHash - returns byte array
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string

        var buider = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            buider.Append(bytes[i].ToString("x2"));
        }
        return buider.ToString();
    }

    public static class PermissionHelper
    {
        public static string GetPermission(string functionCode, string commandCode)
            => string.Join(".", functionCode, commandCode);
    }

    public static class PermissionAuthorize
    {
        public static string GetPermission(EFunctionCode functionCode, ECommandCode commandCode)
            => string.Join(".", functionCode, commandCode);
    }
    
    public static string getDataFromToken( string token, string name)
    {
        try
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            string value = tokenS.Claims.First(claim => claim.Type == name).Value;
            return value;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

}