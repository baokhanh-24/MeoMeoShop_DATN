using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MeoMeo.Contract.DTOs.Employees
{
    public class UploadAvatarDTO
    {
        [Required(ErrorMessage = "File avatar là bắt buộc")]
        public IFormFile AvatarFile { get; set; } = null!;
    }
}
