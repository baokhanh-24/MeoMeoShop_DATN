using System;

namespace MeoMeo.Contract.DTOs.Statistics
{
    public class TopProductsRequestDTO
    {
        public StatisticsPeriod Period { get; set; } = StatisticsPeriod.Daily;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
