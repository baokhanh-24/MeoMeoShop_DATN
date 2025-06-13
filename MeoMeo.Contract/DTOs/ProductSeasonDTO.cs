using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class ProductSeasonDTO
    {
        public Guid SeasonId { get; set; }
        public Guid ProductId { get; set; }
    }
}
