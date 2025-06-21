using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs
{
    public class CartResponseDTO : BaseResponse
    {
        public Guid Id { get; set; }
        public Guid CustomersId { get; set; }
        public decimal TongTien { get; set; }
        public DateTime NgayTao { get; set; }
        public Guid createBy { get; set; }
        public DateTime? lastModificationTime { get; set; }
    }
}
