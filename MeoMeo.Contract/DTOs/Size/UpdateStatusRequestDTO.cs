using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Size
{
    public class UpdateSizeStatusRequestDTO
    {
        public Guid Id { get; set; }
        public int Status { get; set; } 
    }
}
