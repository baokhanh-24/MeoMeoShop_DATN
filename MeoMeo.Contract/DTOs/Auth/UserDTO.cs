namespace MeoMeo.Contract.DTOs.Auth;

public class UserDTO
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string PasswordHash { get; set; }
    public string Avatar { get; set; }
    public DateTime LastLogin { get; set; }
    public string Email { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedEndDate { get; set; }
    public int Status { get; set; }
}