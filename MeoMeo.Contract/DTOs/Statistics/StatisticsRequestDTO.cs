using System;

namespace MeoMeo.Contract.DTOs.Statistics
{
    public class StatisticsRequestDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public StatisticsPeriod Period { get; set; } = StatisticsPeriod.Daily;
    }

    public enum StatisticsPeriod
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Custom
    }
}
