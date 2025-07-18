using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.PromotionDetail
{
    public class GetListPromotionDetailRequestDTO : BasePaging
    {
        public Guid PromotionIdFilter { get; set; }
        public Guid ProductDetailIdFilter { get; set; }
        public float? DiscountFilter { get; set; }
        public string? NoteFilter { get; set; }
    }
}
