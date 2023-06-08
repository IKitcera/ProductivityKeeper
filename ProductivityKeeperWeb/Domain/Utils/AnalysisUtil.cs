using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductivityKeeperWeb.Domain.Utils
{
    public class AnalysisUtil
    {
        public static UserStatistic CountStatistic(Unit unit, UserStatistic statistic)
        {
            if (unit.Categories.Count == 0 || unit.Categories.SelectMany(c => c.Subcategories).Count() == 0)
                return statistic;


            List<TaskItem> AllTasks = unit.Categories
                    .SelectMany(ctg => ctg.Subcategories
                    .SelectMany(sub => sub.Tasks))
                    .ToList();

            List<TaskItem> ConnectedTasks = AllTasks
                .Where(task => task.Subcategories.Count > 1)
                .ToList(); // todo: fix

            var singleTasks = AllTasks
                .Where(task => task.Subcategories.Count == 1)
                .ToList();

            statistic.PercentOfDoneTotal = AllTasks.Count > 0 ?
                (float)AllTasks.Where(t => t.IsChecked).Count() / AllTasks.Count :
                0;

            statistic.CountOfDoneToday = AllTasks
                .Where(t => t.DoneDate?.Date == DateTime.Now.Date)
                .Count();

            int countOnToday = AllTasks
                .Where(t => t.Deadline.HasValue && t.Deadline.Value.Date == DateTime.Now.Date)
                .Count();
            int countInTodaySub = unit.Categories
                .SelectMany(c => c.Subcategories)
                .Where(s => s.Name == "Today")
                .SelectMany(s => s.Tasks)
                .Where(t => (t.Deadline.HasValue && t.Deadline.Value.Date != DateTime.Now.Date)
                || (!t.Deadline.HasValue))
                .Count();
            statistic.TasksOnToday = countOnToday + countInTodaySub;
            statistic.AllTasksCount = AllTasks.Count;

            statistic.PercentOfDoneToday = statistic.TasksOnToday != 0 ?
                (float)statistic.CountOfDoneToday / statistic.TasksOnToday :
                (statistic.CountOfDoneToday >= 1 ? 1 : 0);

            // With deadline and was done later of was not done later
            statistic.CountOfExpiredTotal = AllTasks
                .Where(t => t.Deadline.HasValue && ((t.DoneDate.HasValue && t.DoneDate.Value > t.Deadline.Value)
                || (DateTime.Now > t.Deadline.Value && !t.IsChecked)))
                .Count();
            statistic.CountOfDoneTotal = AllTasks
                .Where(t => t.IsChecked)
                .Count();

            statistic.PerDayStatistic = CountDonePerDayStatistic(AllTasks);

            UpdateDonePerDayStatisticWithTasksFromArchive(statistic.PerDayStatistic, unit.TaskArchive.ToList());
            UnionWithArchivedTasks(statistic, unit);
            statistic.PerDayStatistic = statistic.PerDayStatistic.OrderBy(x => x.Date).ToList();

            return statistic;
        }

        private static List<DonePerDay> CountDonePerDayStatistic(List<TaskItem> AllTasks)
        {
            List<DonePerDay> perDayStatistic = new();

            Dictionary<DateTime?, List<TaskItem>> grouppedByDoneDate = AllTasks.Where(t => t.DoneDate.HasValue && t.IsChecked).GroupBy(task => task.DoneDate?.Date)?.ToDictionary(x => x.Key, x => x.ToList());

            foreach (DateTime? date in grouppedByDoneDate.Keys)
            {
                int doneThatDay = grouppedByDoneDate[date].Count;
                perDayStatistic.Add(new DonePerDay { Date = date.Value, CountOfDone = doneThatDay });
            }

            return perDayStatistic;
        }

        private static void UpdateDonePerDayStatisticWithTasksFromArchive(List<DonePerDay> perDayStat, List<ArchivedTask> archivedTasks)
        {
            List<DonePerDay> perDayStatistic = new();

            Dictionary<DateTime?, List<ArchivedTask>> grouppedByDoneDate = archivedTasks
                .Where(t => t.DoneDate.HasValue && t.Status == ArchievedTaskStatus.Done)
                .GroupBy(task => task.DoneDate?.Date)
                ?.ToDictionary(x => x.Key, x => x.ToList());

            foreach (DateTime? date in grouppedByDoneDate.Keys)
            {
                int doneThatDay = grouppedByDoneDate[date].Count();
                perDayStatistic.Add(new DonePerDay { Date = date.Value, CountOfDone = doneThatDay });
            }

            // union

            if (!perDayStat.Any())
            {
                perDayStat = perDayStatistic;
                return;
            }

            foreach (DonePerDay archivedSts in perDayStatistic)
            {
                DonePerDay existingMatch = perDayStat.FirstOrDefault(s => s.Date == archivedSts.Date);
                if (existingMatch != null)
                {
                    existingMatch.CountOfDone = archivedSts.CountOfDone + existingMatch.CountOfDone; //total
                }
                else
                {
                    perDayStat.Add(archivedSts);
                }
            }
        }

        private static void UnionWithArchivedTasks(UserStatistic statistic, Unit unit)
        {
            if (!unit.TaskArchive.Any())
            {
                return;
            }

            int allTasksCount = unit.TaskArchive.Count;
            List<ArchivedTask> doneTasks = unit.TaskArchive.Where(t => t.Status == ArchievedTaskStatus.Done).ToList();

            int tasksOnToday = unit.TaskArchive
                .Where(t => t.Deadline.HasValue && t.Deadline.Value.Date == DateTime.Now.Date)
                .Count();
            int doneTodayTasksCount = doneTasks
                .Where(t => t.DoneDate.HasValue && t.DoneDate.Value.Date == DateTime.Now.Date)
                .Count();

            float percentOfDoneToday = tasksOnToday != 0 ?
                (float)doneTodayTasksCount / tasksOnToday :
                (doneTodayTasksCount >= 1 ? 1 : 0);

            float percentOfDoneTotal = doneTasks.Count > 0 ? (float)doneTasks.Count / allTasksCount : 0;

            int expiredTasksCount = unit.TaskArchive.Where(t => t.Status == ArchievedTaskStatus.Expired).Count();

            // Formula: ' += fraction of tasks * percentage '
            int commonDenominatorOfToday = statistic.TasksOnToday + tasksOnToday;
            if (statistic.TasksOnToday != 0 && tasksOnToday != 0)              // this is usual case
            {
                statistic.PercentOfDoneToday = (percentOfDoneToday * ((float)tasksOnToday / commonDenominatorOfToday)) +
                                               (statistic.PercentOfDoneToday * ((float)statistic.TasksOnToday / commonDenominatorOfToday));
            }
            else
            {
                statistic.PercentOfDoneToday = statistic.TasksOnToday == 0 && tasksOnToday == 0
                ? statistic.CountOfDoneToday + doneTodayTasksCount > 0 ? 1 : 0
                : (float)(statistic.CountOfDoneToday + doneTodayTasksCount) / (statistic.TasksOnToday + tasksOnToday);
            }

            int commonDenominatorOfTotal = allTasksCount + statistic.AllTasksCount;
            if (statistic.AllTasksCount != 0 && allTasksCount != 0)
            {
                statistic.PercentOfDoneTotal = (percentOfDoneTotal * ((float)allTasksCount / commonDenominatorOfTotal)) +
                                               (statistic.PercentOfDoneTotal * ((float)statistic.AllTasksCount / commonDenominatorOfTotal));
            }
            else if (statistic.AllTasksCount == 0 || allTasksCount == 0)
            {
                statistic.PercentOfDoneTotal = (float)(statistic.CountOfDoneTotal + doneTasks.Count) / (statistic.AllTasksCount + allTasksCount);
            }

            statistic.CountOfDoneToday += doneTodayTasksCount;
            statistic.CountOfDoneTotal += doneTasks.Count;
            statistic.CountOfExpiredTotal += expiredTasksCount;
        }
    }
}
