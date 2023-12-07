using System.Collections.Generic;

namespace ProductivityKeeperWeb.Domain.DTO
{
    public class AverageStatisticDTO
    {
        public IEnumerable<float> AverageUsersStatistic { get; set; }
        public IEnumerable<int> TodayUsersStatistic { get; set; }
        public float ActiveUserAverage { get; set; }
        public int ActiveUserToday { get; set; }
    }
}
