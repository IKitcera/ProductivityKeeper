using Microsoft.AspNetCore.Http;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ProductivityKeeperWeb.Services
{
    public class AnalyticService: IAnalytics
    {
        private readonly ApplicationContext _context;
        private readonly Unit unit;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AnalyticService(ApplicationContext _context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = _context;
            _httpContextAccessor = httpContextAccessor;
            this.unit = _context.Units.FirstOrDefault(u => u.UserId == httpContextAccessor.HttpContext.User.Identity.Name);
        }
        public async System.Threading.Tasks.Task<UserStatistic> GetStatistic()
        {
            UserStatistic statistic = new UserStatistic();

            if (unit.Categories.Any() && unit.Categories.Select(c => c.Subcategories).Any())
            {
                List<Task> AllTasks = unit.Categories
                    .SelectMany(ctg => ctg.Subcategories
                    .SelectMany(sub => sub.Tasks)).ToList();

                List<Task> ConnectedTasks = unit.TaskToManySubcategories
                    .SelectMany(list => list.TaskSubcategories.Skip(1)
                    .Select(ts => unit.Categories
                    .FirstOrDefault(c => c.Id == ts.CategoryId)?
                    .Subcategories.FirstOrDefault(s => s.Id == ts.SubcategoryId)?.Tasks?
                    .FirstOrDefault(t => t.Id == ts.TaskId))).ToList();

                AllTasks = AllTasks.Where(t => !ConnectedTasks.Select(ct => ct.Id).Contains(t.Id)).ToList();


                statistic.PercentOfDoneTotal = AllTasks.Count > 0 ? (float)(AllTasks.Where(t => t.IsChecked).Count()) / AllTasks.Count : 0;

                statistic.CountOfDoneToday = AllTasks.Where(t => t.DoneDate?.Date == DateTime.Now.Date).Count();

                var countOnToday = AllTasks.Where(t => t.Deadline.HasValue && t.Deadline.Value.Date == DateTime.Now.Date).Count();
                var countInTodaySub = unit.Categories.SelectMany(c => c.Subcategories).Where(s => s.Name == "Today")
                    .SelectMany(s => s.Tasks).Where(t => (t.Deadline.HasValue && t.Deadline.Value.Date != DateTime.Now.Date) ||
                    (!t.Deadline.HasValue)).Count();
                statistic.TasksOnToday = countOnToday + countInTodaySub;
                statistic.AllTasksCount = AllTasks.Count;

                statistic.PercentOfDoneToday = statistic.TasksOnToday != 0 ?
                    (float)statistic.CountOfDoneToday / statistic.TasksOnToday :
                    (statistic.CountOfDoneToday >= 1 ? 1 : 0);


                statistic.CountOfExpiredTotal = AllTasks.Where(t => t.DoneDate?.Date > t.Deadline?.Date || (DateTime.Now.Date > t.Deadline?.Date && !t.IsChecked)).Count();
                statistic.CountOfDoneTotal = AllTasks.Where(t => t.IsChecked).Count();

                statistic.PerDayStatistic = CountDonePerDayStatistic(AllTasks);

                statistic.PerDayStatistic = unit.Statistic?.PerDayStatistic?.Union(statistic.PerDayStatistic).ToList() ?? statistic.PerDayStatistic;
            }

            UnionWithArchivedTasks(statistic);

            statistic.PerDayStatistic?.Reverse();
            unit.Statistic = statistic;
            
            await _context.SaveChangesAsync();
           
            if (!statistic.PerDayStatistic.Any() ||
                statistic.PerDayStatistic.Last().Date.Date != DateTime.Now.Date)
            {
                statistic.PerDayStatistic.Add(new DonePerDay { Date = DateTime.Now.Date, CountOfDone = 0 });
            }
                return statistic;
        }

        private List<DonePerDay> CountDonePerDayStatistic (List<Task> AllTasks)
        {
            List<DonePerDay> perDayStatistic = new List<DonePerDay>();  

            var grouppedByDoneDate = AllTasks.Where(t => t.DoneDate.HasValue && t.IsChecked).GroupBy(task => task.DoneDate?.Date)?.ToDictionary(x => x.Key, x => x.ToList());

            foreach (var date in grouppedByDoneDate.Keys)
            {
                var doneThatDay = grouppedByDoneDate[date].Count;
                perDayStatistic.Add(new DonePerDay { Date = date.Value, CountOfDone = doneThatDay });
            }

            return perDayStatistic;
        }

        private List<DonePerDay> UpdateDonePerDayStatisticWithTasksFromArchive(List<ArchivedTask> archivedTasks)
        {
            List<DonePerDay> perDayStatistic = new List<DonePerDay>();

            var grouppedByDoneDate = archivedTasks
                .Where(t => t.DoneDate.HasValue && t.Status == ArchievedTaskStatus.Done)
                .GroupBy(task => task.DoneDate?.Date)?.ToDictionary(x => x.Key, x => x.ToList());

            foreach (var date in grouppedByDoneDate.Keys)
            {
                var doneThatDay = grouppedByDoneDate[date].Count;
                perDayStatistic.Add(new DonePerDay { Date = date.Value, CountOfDone = doneThatDay });
            }

            return perDayStatistic;
        }

        private void UnionWithArchivedTasks(UserStatistic statistic)
        {
            if (!unit.TaskArchive.Any())
                return;

            var allTasksCount = unit.TaskArchive.Count;
            var doneTasks = unit.TaskArchive.Where(t => t.Status == ArchievedTaskStatus.Done).ToList();

            var tasksOnToday = unit.TaskArchive.Where(t => t.Deadline.HasValue && t.Deadline.Value.Date == DateTime.Now.Date).Count();
            var doneTodayTasksCount = doneTasks.Where(t => t.DoneDate.HasValue && t.DoneDate.Value.Date == DateTime.Now.Date).Count();

            var percentOfDoneToday = tasksOnToday != 0 ?
                (float)doneTodayTasksCount / tasksOnToday :
                (doneTodayTasksCount >= 1 ? 1 : 0);

            var percentOfDoneTotal = doneTasks.Count > 0 ? (float)(doneTasks.Count) / allTasksCount : 0;

            var expiredTasksCount = unit.TaskArchive.Where(t => t.Status == ArchievedTaskStatus.Expired).Count();

            // Formula: ' += fraction of tasks * percentage '
            var commonDenominatorOfToday = statistic.TasksOnToday + tasksOnToday;
            if (statistic.TasksOnToday != 0 && tasksOnToday != 0)              // this is usual case
                statistic.PercentOfDoneToday = (percentOfDoneToday * ((float)tasksOnToday / commonDenominatorOfToday)) +
                                               (statistic.PercentOfDoneToday * ((float)statistic.TasksOnToday / commonDenominatorOfToday));
            else if (statistic.TasksOnToday == 0 && tasksOnToday == 0)         // this is special case 1
                statistic.PercentOfDoneToday = statistic.CountOfDoneToday + doneTodayTasksCount > 0 ? 1 : 0;
            else                                                               // this is special case 2 (one of them = 0)
                statistic.PercentOfDoneToday = (float)(statistic.CountOfDoneToday + doneTodayTasksCount) / (statistic.TasksOnToday + tasksOnToday);

            var commonDenominatorOfTotal = allTasksCount + statistic.AllTasksCount;
            if (statistic.AllTasksCount != 0 && allTasksCount != 0)
                statistic.PercentOfDoneTotal = (percentOfDoneTotal * ((float)allTasksCount / commonDenominatorOfTotal)) + 
                                               (statistic.PercentOfDoneTotal * ((float)statistic.AllTasksCount / commonDenominatorOfTotal));
            else if (statistic.AllTasksCount == 0 || allTasksCount == 0)
                statistic.PercentOfDoneTotal = (float)(statistic.CountOfDoneTotal + doneTasks.Count) / (statistic.AllTasksCount + allTasksCount);

            statistic.CountOfDoneToday += doneTodayTasksCount;
            statistic.CountOfDoneTotal += doneTasks.Count;
            statistic.CountOfExpiredTotal += expiredTasksCount;
        }
    }
}
