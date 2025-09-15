using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Promotion
{
    public class GetListPromotionRequestDTO : BasePaging
    {
        public string? TitleFilter { get; set; }
        public DateOnly? StartDateFilter { get; set; }
        public DateOnly? EndDateFilter { get; set; }
        public string? DescriptionFilter { get; set; }
    }
}
