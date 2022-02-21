using ProductivityKeeperWeb.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductivityKeeperWeb.Models
{
    public class UserStatistic
    {
        public int Id { get; set; }
        public Dictionary<DateTime, float> DonePerDay = new Dictionary<DateTime, float>();
        public float PercentOfDoneToday { get; set; }
        public float PercentOfDoneTotal { get; set; }
        public int CountOfDoneToday { get; set; }
        public int CountOfDoneTotal { get; set; }
        public int CountOfExpiredTotal { get; set; }

    }
}
