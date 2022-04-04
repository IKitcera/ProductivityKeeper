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

            statistic.PercentOfDoneToday = countOnToday + countInTodaySub != 0 ?
                statistic.CountOfDoneToday / (countOnToday + countInTodaySub) :
                (statistic.CountOfDoneToday >=1 ? 1 : 0);


            statistic.CountOfExpiredTotal = AllTasks.Where(t => t.DoneDate > t.Deadline || (DateTime.Now > t.Deadline && !t.IsChecked)).Count();
            statistic.CountOfDoneTotal = AllTasks.Where(t => t.IsChecked).Count();

            statistic.PerDayStatistic = CountDonePerDay(AllTasks);

            statistic.PerDayStatistic = unit.Statistic?.PerDayStatistic?.Union(statistic.PerDayStatistic).ToList() ?? statistic.PerDayStatistic;
            statistic.PerDayStatistic.Reverse();
            unit.Statistic = statistic;
            await _context.SaveChangesAsync();

            if (!statistic.PerDayStatistic.Any() ||
                statistic.PerDayStatistic.Last().Date.Date != DateTime.Now.Date)
            {
                statistic.PerDayStatistic.Add(new DonePerDay { Date = DateTime.Now.Date, CountOfDone = 0 });
            }
                return statistic;
        }

        private List<DonePerDay> CountDonePerDay (List<Task> AllTasks)
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
    }
}
