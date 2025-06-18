using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class BrandDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime EstablishYear { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
    }
}
