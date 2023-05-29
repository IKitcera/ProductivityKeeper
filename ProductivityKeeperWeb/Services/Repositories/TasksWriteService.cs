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

namespace ProductivityKeeperWeb.Services.Repositories
{
    [Authorize]
    public class TasksWriteService : ITasksWriteService
    {
        private readonly ApplicationContext _context;
        private readonly ITasksReadService _tasksReadService;
        public TasksWriteService(ApplicationContext context, ITasksReadService tasksReadService)
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
            return await _tasksReadService.GetTask(task.Id);
        }

        public async Task DeleteTaskItem(int taskId)
        {
            var item = await _context.Tasks.FindAsync(taskId);

            _context.Entry(item).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        // Statistic
        public async Task<UserStatistic> UpdateUserStatistic(UserStatistic statistic)
        {
            var item = await _context.Statistics.FindAsync(statistic);

            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return item;
        }
    }
}
