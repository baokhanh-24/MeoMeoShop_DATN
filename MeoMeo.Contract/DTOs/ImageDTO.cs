using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class ImageDTO
    {
        public Guid? Id { get; set; }
        public Guid ProductDetailId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Url { get; set; }
    }
}
