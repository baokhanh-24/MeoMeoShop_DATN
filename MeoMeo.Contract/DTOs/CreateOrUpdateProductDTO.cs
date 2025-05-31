using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CreateOrUpdateProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
