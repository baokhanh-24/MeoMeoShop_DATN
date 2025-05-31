using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.Commons
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
    }
}
