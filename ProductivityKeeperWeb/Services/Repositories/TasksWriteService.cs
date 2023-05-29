using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using ProductivityKeeperWeb.Domain.Utils;
using ProductivityKeeperWeb.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace ProductivityKeeperWeb.Services.Repositories
{
    [Authorize]
    public class TasksWriteService : ITasksWriteService
    {
        private readonly ApplicationContext _context;
        private readonly ITasksReadService _tasksReadService;
        public TasksWriteService(
            ApplicationContext context,
            ITasksReadService tasksReadService
            )
        {
            _context = context;
            _tasksReadService = tasksReadService;
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
            ctg.Name = category.Name;
            ctg.ColorHex = category.ColorHex;
            ctg.Subcategories = category.Subcategories;

            _context.Entry(ctg).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategory(int categoryId)
        {
            var item = await _context.Categories.FindAsync(categoryId);

            _context.Entry(item).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
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

        public async Task DeleteSubcategory(int subcategoryId)
        {
            var item = await _context.Subcategories.FindAsync(subcategoryId);

            _context.Entry(item).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        // TaskItem 
        public async Task<TaskItem> AddTaskItem(TaskItem task)
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
            return await _tasksReadService.GetTask(task.Id);
        }

        public async Task<TaskItem> UpdateTaskItem(TaskItem task)
        {
            var item = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            if (item == null)
                throw new ArgumentNullException($"Task was not found with id {task.Id}");

            item.Text = task.Text;
            item.Deadline = task.Deadline?.ToLocalTime();
            item.DoneDate = task.IsChecked ? DateTime.Now : null;
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
                var completed = item.GoalRepeatCount - item.TimesToRepeat;

                item.GoalRepeatCount = task.GoalRepeatCount;
                item.TimesToRepeat = completed > 0 ? task.GoalRepeatCount - completed : task.GoalRepeatCount;
                item.HabbitIntervalInHours = task.HabbitIntervalInHours;
            }
            else
            {
                item.GoalRepeatCount = null;
                item.TimesToRepeat = null;
                item.HabbitIntervalInHours = null;
            }

            await _context.SaveChangesAsync();
            return await _tasksReadService.GetTask(task.Id); // Think about logic separation to avoid here this service
        }

        public async Task<TaskItem> ChangeTaskStatus(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            task.IsChecked = !task.IsChecked;
            task.DoneDate = task.IsChecked ? DateTime.Now : null;


            if (task.IsRepeatable && task.IsChecked)
            {
                task.TimesToRepeat = task.TimesToRepeat - 1 > 0 ? task.TimesToRepeat - 1 : 0;
            }

            await _context.SaveChangesAsync();
            return await _tasksReadService.GetTask(task.Id); // same
        }

        public async Task DeleteTaskItem(int taskId, int unitId)
        {
            var item = await _context.Tasks.FindAsync(taskId);

            _context.Entry(item).State = EntityState.Deleted;

            var archievedTask = new ArchivedTask { UnitId = unitId };
            if (item.IsChecked)
            {
                archievedTask.Status = ArchievedTaskStatus.Done;
                archievedTask.DoneDate = item.DoneDate;
            }
            else if (item.Deadline.HasValue && DateTime.Now.Date > item.Deadline.Value)
            {
                archievedTask.Status = ArchievedTaskStatus.Expired;
            }
            else
            {
                archievedTask.Status = ArchievedTaskStatus.Undone;
            }
            archievedTask.Deadline = item.Deadline;
           
            await _context.ArchivedTasks.AddAsync(archievedTask);

            await _context.SaveChangesAsync();
        }

        // Statistic
        public async Task<UserStatistic> UpdateUserStatistic(UserStatistic statistic)
        {
            var item = await _context.Statistics.FindAsync(statistic.Id);

            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return item;
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

                _context.Entry(unit).State = EntityState.Modified;

                var statToUpdate = await _context.Statistics.FirstOrDefaultAsync(x => x.Id == unit.Statistic.Id);
                AnalysisUtil.CountStatistic(unit, statToUpdate);

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
    }
}
