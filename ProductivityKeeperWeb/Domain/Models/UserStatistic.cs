using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey(nameof(UnitId))]
        public int UnitId { get; set; }

        // Won't be writtent to db
        public int TasksOnToday;
        public int AllTasksCount;

    }
}
