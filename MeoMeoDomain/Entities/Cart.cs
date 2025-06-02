using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime LastModificationTime { get; set; }
    }
}
