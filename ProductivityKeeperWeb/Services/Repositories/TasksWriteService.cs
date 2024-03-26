using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using ProductivityKeeperWeb.Domain.Utils;
using ProductivityKeeperWeb.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IStatistics = ProductivityKeeperWeb.Domain.Interfaces.IStatistics;

namespace ProductivityKeeperWeb.Services.Repositories
{
    [Authorize]
    public class TasksWriteService : ITasksWriteService
    {
        private readonly ApplicationContext _context;
        private readonly ITasksReadService _tasksReadService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IAuthService _authService;

        public TasksWriteService(
            ApplicationContext context,
            ITasksReadService tasksReadService,
            IBackgroundJobClient backgroundJobClient,
            IAuthService authService
            )
        {
            _context = context;
            _tasksReadService = tasksReadService;
            _backgroundJobClient = backgroundJobClient;
            _authService = authService;
        }

        // Category 
        public async Task<Category> AddCategory(Category category)
        {
            var categories = await _context.Categories.ToListAsync();
            category = TaskRelatedInitializar.FillCategory(category, categories);
            var ctg = await _context.Categories.AddAsync(category);

            _context.Entry(ctg.Entity).State = EntityState.Added;

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            var ctg = await _context.Categories.FindAsync(category.Id);

            // Dangerous place
            _context.Entry(ctg).State = EntityState.Detached;

            ctg.Name = category.Name;
            ctg.ColorHex = category.ColorHex;
            ctg.Subcategories = category.Subcategories;
            
            _context.Entry(ctg).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategory(int categoryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var item = await _context.Categories.FindAsync(categoryId);

                _context.Entry(item).State = EntityState.Deleted;

                await _context.SaveChangesAsync();

                RunBackgroundUpdateStatisticJob(_authService.GetUnitId());

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        // Subcategory 
        public async Task<Subcategory> AddSubcategory(Subcategory subcategory)
        {
            var allSubs = await _context.Subcategories
                .Where(sub => sub.CategoryId == subcategory.CategoryId)
                .ToListAsync();
            subcategory = TaskRelatedInitializar.FillSubcategory(subcategory, allSubs);
            var item = await _context.Subcategories.AddAsync(subcategory);

            _context.Entry(item.Entity).State = EntityState.Added;

            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task<Subcategory> UpdateSubcategory(Subcategory subcategory)
        {
            var item = await _context.Subcategories.FindAsync(subcategory.Id);
            item.Name = subcategory.Name;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task ReorderSubcategories(IEnumerable<int> ids)
        {
            var targetSubs = await _context.Subcategories
                .Where(sub => ids.Contains(sub.Id))
                .ToListAsync();

            for (int i = 0; i < ids.Count(); i++)
            {
                var match = targetSubs.First(x => x.Id == ids.ElementAt(i));
                match.Position = i;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ReorderCategories(IEnumerable<Category> categories)
        {
            var ids = categories.Select(x => x.Id);

            var targetCategories = await _context.Categories
                .Where(cat => ids.Contains(cat.Id))
                .ToListAsync();

            int i = 0;
            foreach (var category in categories)
            {
                var match = targetCategories.First(x => x.Id == category.Id);
                match.Position = i;
                match.IsVisible = category.IsVisible;
                match.ColorHex = category.ColorHex;
                i++;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ReorderTasks(IEnumerable<int> ids)
        {
            var targetTasks = await _context.Tasks
               .Where(task => ids.Contains(task.Id))
               .ToListAsync();

            for (int i = 0; i < ids.Count(); i++)
            {
                var match = targetTasks.First(x => x.Id == ids.ElementAt(i));
                match.Position = i;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubcategory(int subcategoryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var item = await _context.Subcategories.FindAsync(subcategoryId);

                _context.Entry(item).State = EntityState.Deleted;

                await _context.SaveChangesAsync();

                RunBackgroundUpdateStatisticJob(_authService.GetUnitId());

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }
        }

        // TaskItem 
        public async Task<TaskItem> AddTaskItem(TaskItem task)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                task = TaskRelatedInitializar.FillTask(task);

                var subcategoryItem = await _context.Subcategories.FindAsync(task.Subcategories[0].Id);

                if (subcategoryItem == null)
                    throw new InvalidOperationException("Cannot find category id!");

                task.Subcategories[0] = subcategoryItem;
                var item = await _context.Tasks.AddAsync(task);

                _context.Entry(subcategoryItem).State = EntityState.Modified;
                _context.Entry(item.Entity).State = EntityState.Added;

                await _context.SaveChangesAsync();

                RunBackgroundUpdateStatisticJob(_authService.GetUnitId());

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }

            return await _tasksReadService.GetTask(task.Id);
        }

        public async Task<TaskItem> UpdateTaskItem(TaskItem task)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var item = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id);

                if (item == null)
                    throw new ArgumentNullException($"Task was not found with id {task.Id}");

                item.Text = task.Text;
                item.Deadline = task.Deadline?.ToLocalTime();
                item.DoneDate = task.IsChecked ? DateTime.Now : null;
                item.ExecutionDuration = task.ExecutionDuration;
                item.IsChecked = task.IsChecked;

                item.Subcategories = await _context.Subcategories
                    .Where(sub => task.Subcategories.Select(s => s.Id).Contains(sub.Id))
                    .ToListAsync();

                var relationsMap = await _context.SubcategoriesTasks.AsNoTracking()
                    .Where(sc => sc.TaskItemId == task.Id)
                    .ToListAsync();
                _context.SubcategoriesTasks.RemoveRange(relationsMap);

                if (task.Subcategories?.Any() ?? false)
                {
                    _context.SubcategoriesTasks.AddRange(
                        task.Subcategories.Select(subcategory =>
                        new SubcategoryTask
                        {
                            SubcategoryId = subcategory.Id,
                            TaskItemId = item.Id
                        }));
                }

                item.IsRepeatable = task.IsRepeatable;

                if (task.IsRepeatable)
                {
                    item.GoalRepeatCount = task.GoalRepeatCount;
                    item.TimesToRepeat = task.TimesToRepeat;
                    item.HabbitIntervalInHours = task.HabbitIntervalInHours;
                }
                else
                {
                    item.GoalRepeatCount = null;
                    item.TimesToRepeat = null;
                    item.HabbitIntervalInHours = null;
                }

                await _context.SaveChangesAsync();

                RunBackgroundUpdateStatisticJob(_authService.GetUnitId());

                await transaction.CommitAsync();

                return await _tasksReadService.GetTask(task.Id); // Think about logic separation to avoid here this service
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TaskItem> ChangeTaskStatus(int taskId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var task = await _context.Tasks.FindAsync(taskId);
                task.IsChecked = !task.IsChecked;
                task.DoneDate = task.IsChecked ? DateTime.Now : null;


                if (task.IsRepeatable && task.IsChecked)
                {
                    task.TimesToRepeat = task.TimesToRepeat - 1 > 0 ? task.TimesToRepeat - 1 : 0;
                }

                await _context.SaveChangesAsync();

                RunBackgroundUpdateStatisticJob(_authService.GetUnitId());

                await transaction.CommitAsync();

                return await _tasksReadService.GetTask(task.Id); // same
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteTaskItem(int taskId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var unitId = _authService.GetUnitId();
                var item = await _context.Tasks.FindAsync(taskId);

                _context.Entry(item).State = EntityState.Deleted;

                var archievedTask = new ArchivedTask { UnitId = unitId };
                archievedTask.DoneDate = item.DoneDate;
                archievedTask.Deadline = item.Deadline;

                if (item.IsChecked)
                    archievedTask.Status = ArchievedTaskStatus.Done;
                else if (item.Deadline.HasValue && DateTime.Now.Date > item.Deadline.Value)
                    archievedTask.Status = ArchievedTaskStatus.Expired;
                else
                    archievedTask.Status = ArchievedTaskStatus.Undone;

                await _context.ArchivedTasks.AddAsync(archievedTask);

                await _context.SaveChangesAsync();

                RunBackgroundUpdateStatisticJob(unitId);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Statistic
        // TODO: Move
        public async Task<UserStatistic> UpdateUserStatistic(UserStatistic statistic)
        {
            var item = await _context.Statistics.Include(s => s.PerDayStatistic)
                .FirstOrDefaultAsync(x => x.Id == statistic.Id);

            item.CountOfDoneToday = statistic.CountOfDoneToday;
            item.CountOfDoneTotal = statistic.CountOfDoneTotal;
            item.CountOfExpiredTotal = statistic.CountOfExpiredTotal;
            item.PercentOfDoneToday = statistic.PercentOfDoneToday;
            item.PercentOfDoneTotal = statistic.PercentOfDoneTotal;
            item.PerDayStatistic = statistic.PerDayStatistic;

            _context.Statistics.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<UserStatistic> FillNewStatistic(Unit unit)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var statistic = new UserStatistic { UnitId = unit.Id };
                var res = await _context.Statistics.AddAsync(statistic);
                await _context.SaveChangesAsync();

                unit.StatisticId = statistic.Id;

                _context.Units.Entry(unit).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return statistic;
            }
            catch (Exception)
            {
                // Rollback the transaction in case of any exception
                await transaction.RollbackAsync();
                throw; // Rethrow the exception to be handled at a higher level
            }
        }

        public async Task<Unit> AddUnitForNewCommer(string email)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var unit = new Unit { UserId = email };
                unit = TaskRelatedInitializar.FillUnitForNewcommer(unit);

                await _context.Units.AddAsync(unit);

                await _context.SaveChangesAsync();

                unit.StatisticId = unit.Statistic.Id;
                unit.TimerId = unit.Timer.Id;

                _context.Units.Entry(unit).State = EntityState.Modified;

                var statToUpdate = await _context.Statistics.FirstOrDefaultAsync(x => x.Id == unit.Statistic.Id);
                StatisticsUtil.CountBaseStatistic(unit, statToUpdate);

                _context.Statistics.Entry(statToUpdate).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Rollback the transaction in case of any exception
                await transaction.RollbackAsync();
                throw; // Rethrow the exception to be handled at a higher level
            }
            return await _context.Units.FirstOrDefaultAsync(u => u.UserId == email);
        }

        public string RunBackgroundUpdateStatisticJob(int unitId)
        {
            return _backgroundJobClient.Enqueue<IStatistics>(x => x.CountStatistic(unitId));
        }

    }
}
