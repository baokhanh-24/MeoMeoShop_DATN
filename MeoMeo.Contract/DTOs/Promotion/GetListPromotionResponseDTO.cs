using MeoMeo.Contract.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Contract.DTOs.Promotion
{
    public class GetListPromotionResponseDTO:BaseResponse
    {
        public int TotalAll { get; set; }
        public int Draft { get; set; }
        public int NotHappenedYet { get; set; }
        public int IsGoingOn { get; set; }
        public int Ended { get; set; }
    }
}
