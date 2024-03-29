﻿using ProductivityKeeperWeb.Domain.DTO;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductivityKeeperWeb.Domain.Utils
{
    public class StatisticsUtil
    {
        public static UserStatistic CountBaseStatistic(Unit unit, UserStatistic statistic)
        {
            List<TaskItem> AllTasks = unit.Categories
                    .SelectMany(ctg => ctg.Subcategories
                    .SelectMany(sub => sub.Tasks))
                    .DistinctBy(task => task.Id)
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

            statistic.PerDayStatistic = CountDonePerDayStatistic(AllTasks, statistic.Id);

            UpdateDonePerDayStatisticWithTasksFromArchive(statistic, unit.TaskArchive.ToList());
            UnionWithArchivedTasks(statistic, unit);
            statistic.PerDayStatistic = statistic.PerDayStatistic.OrderBy(x => x.Date).ToList();

            return statistic;
        }


        public static AverageStatisticDTO CalculateAverageStatistic(IEnumerable<DonePerDay> perDaysData, int activeUserStatisticId)
        {
            var todaySelector = (IEnumerable<DonePerDay> data) =>
                data.FirstOrDefault(d => d.Date == DateTime.Now.Date)?.CountOfDone ?? 0;
            var avgSelector = (IEnumerable<DonePerDay> data) =>
                (float)data.Sum(x => x.CountOfDone) / data.Count();

            var currentUserData = perDaysData.Where(pdd => pdd.StatisticId == activeUserStatisticId);
            var usersGroupedData = perDaysData.GroupBy(x => x.StatisticId);

            return new AverageStatisticDTO
            {
                ActiveUserToday = todaySelector(currentUserData),
                ActiveUserAverage = avgSelector(currentUserData),
                TodayUsersStatistic = usersGroupedData.Select(group => todaySelector(group)),
                AverageUsersStatistic = usersGroupedData.Select(group => avgSelector(group))
            };
        }


        private static List<DonePerDay> CountDonePerDayStatistic(List<TaskItem> allTasks, int statisticId)
        {
            List<DonePerDay> perDayStatistic = new();

            Dictionary<DateTime?, List<TaskItem>> grouppedByDoneDate = allTasks
                .Where(t => t.DoneDate.HasValue && t.IsChecked)
                .GroupBy(task => task.DoneDate?.Date)
                ?.ToDictionary(x => x.Key, x => x.ToList());

            foreach (DateTime? date in grouppedByDoneDate.Keys)
            {
                int doneThatDay = grouppedByDoneDate[date].Count;
                perDayStatistic.Add(new DonePerDay { Date = date.Value, CountOfDone = doneThatDay, StatisticId = statisticId });
            }

            return perDayStatistic;
        }

        private static void UpdateDonePerDayStatisticWithTasksFromArchive(UserStatistic statistic, List<ArchivedTask> archivedTasks)
        {
            statistic.PerDayStatistic ??= new();

            // Add stat form archive
            Dictionary<DateTime?, List<ArchivedTask>> grouppedByDoneDate = archivedTasks
                .Where(t => t.DoneDate.HasValue && t.Status == ArchievedTaskStatus.Done)
                .GroupBy(task => task.DoneDate?.Date)
                ?.ToDictionary(x => x.Key, x => x.ToList());

            foreach (DateTime? date in grouppedByDoneDate.Keys)
            {
                int doneThatDay = grouppedByDoneDate[date].Count();
                statistic.PerDayStatistic.Add(new DonePerDay { Date = date.Value, CountOfDone = doneThatDay });
            }

            statistic.PerDayStatistic = statistic.PerDayStatistic.GroupBy(stat => stat.Date.Date)
                .Select(x => new DonePerDay
                {
                    Date = x.First().Date,
                    CountOfDone = x.Sum(y => y.CountOfDone)
                }).ToList();
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
