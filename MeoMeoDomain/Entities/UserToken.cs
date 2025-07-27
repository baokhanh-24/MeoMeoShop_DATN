using MeoMeo.Domain.Commons;

namespace MeoMeo.Domain.Entities;
public class UserToken:BaseEntityAudited
{
    public Guid UserId  { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }
    public bool IsUpdateToken { get; set; } 
    public virtual User User { get; set; }
}
