using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductivityKeeperWeb.Services
{
    public class AnalyticService: IAnalytics
    {
        public UserStatistic GetStatistic(Unit usersUnit)
        {
            UserStatistic statistic = new UserStatistic();
            List<Task> AllTasks = new List<Task>();

            usersUnit.Categories.ForEach(c => c.Subcategories.ForEach(s =>
            {
                s.Tasks.ForEach(t => AllTasks.Add(t));
            }));
            statistic.PercentOfDoneTotal = AllTasks.Count > 0 ? AllTasks.Where(t => t.IsChecked).Count() / AllTasks.Count : 0;

            statistic.CountOfDoneToday = AllTasks.Where(t => t.DoneDate?.Date == DateTime.Now.Date).Count();

            var countOnToday = AllTasks.Where(t => t.Deadline.HasValue && t.Deadline.Value.Date == DateTime.Now.Date).Count();
            var countInTodaySub = usersUnit.Categories.SelectMany(c => c.Subcategories).Where(s => s.Name == "Today")
                .SelectMany(s => s.Tasks).Where(t => (t.Deadline.HasValue && t.Deadline.Value.Date != DateTime.Now.Date) ||
                (!t.Deadline.HasValue)).Count();

            statistic.PercentOfDoneToday = countOnToday + countInTodaySub != 0 ?
                statistic.CountOfDoneToday / (countOnToday + countInTodaySub) :
                0;


            statistic.CountOfExpiredTotal = AllTasks.Where(t => t.DoneDate > t.Deadline || (DateTime.Now > t.Deadline && !t.IsChecked)).Count();
            statistic.CountOfDoneTotal = AllTasks.Where(t => t.IsChecked).Count();

            var grouppedByDoneDate = AllTasks.Where(t => t.DoneDate.HasValue && t.IsChecked).GroupBy(task => task.DoneDate)?.ToDictionary(x => x.Key, x => x.ToList());

            statistic.DonePerDay.Clear();

            foreach (var date in grouppedByDoneDate.Keys)
            {
                var tasksOnToday = AllTasks.Where(t => t.Deadline == date).Count();
                var doneToday = grouppedByDoneDate[date].Count;
                statistic.DonePerDay.Add(date.Value, tasksOnToday != 0 ? doneToday / tasksOnToday : 0);
            }

            return statistic;
        }
    }
}
