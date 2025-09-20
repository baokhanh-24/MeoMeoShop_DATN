using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateBrandDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public DateTime EstablishYear { get; set; }
        public int EstablishYearInt
        {
            get => EstablishYear.Year;
            set => EstablishYear = new DateTime(value, 1, 1);
        }
        public string Country { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public IFormFile? LogoFile { get; set; } // Giống như CreateOrUpdateProductDTO có MediaUploads
    }
}
