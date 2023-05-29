using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System.Collections.Generic;

namespace ProductivityKeeperWeb.Domain.Models
{
    [Owned]
    public class UserStatistic
    {
        public int Id { get; set; }
        public List<DonePerDay> PerDayStatistic { get; set; } = new List<DonePerDay>();
        public float PercentOfDoneToday { get; set; }
        public float PercentOfDoneTotal { get; set; }
        public int CountOfDoneToday { get; set; }
        public int CountOfDoneTotal { get; set; }
        public int CountOfExpiredTotal { get; set; }
        public int UnitId { get; set; }
        [JsonIgnore]
        public Unit Unit { get; set; }

        // Won't be writtent to db
        public int TasksOnToday;
        public int AllTasksCount;

    }
}
