using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Material
{
    public class UpdateStatusRequestDTO
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
    }
}
