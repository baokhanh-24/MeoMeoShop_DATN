using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateBrandDTO
    {
        public Guid? Id { get; set; }
        
        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên thương hiệu không được vượt quá 255 ký tự")]
        public string Name { get; set; }
        
        [StringLength(50, ErrorMessage = "Mã thương hiệu không được vượt quá 50 ký tự")]
        public string? Code { get; set; }
        
        [Required(ErrorMessage = "Năm thành lập là bắt buộc")]
        public DateTime EstablishYear { get; set; }
        
        public int EstablishYearInt
        {
            get => EstablishYear.Year;
            set => EstablishYear = new DateTime(value, 1, 1);
        }
        
        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        [StringLength(100, ErrorMessage = "Quốc gia không được vượt quá 100 ký tự")]
        public string Country { get; set; }
        
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }
        
        [StringLength(500, ErrorMessage = "Logo URL không được vượt quá 500 ký tự")]
        public string? Logo { get; set; }
        
        public IFormFile? LogoFile { get; set; } // Giống như CreateOrUpdateProductDTO có MediaUploads
    }
}
