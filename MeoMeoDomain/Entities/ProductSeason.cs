using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class ProductSeason
    {
        public Guid SeasonId { get; set; }
        public Guid ProductId { get; set; }
    }
}
