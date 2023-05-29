using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductivityKeeperWeb.Domain.Utils;

namespace ProductivityKeeperWeb.Services.Repositories
{
    [Authorize]
    public class TasksReadService : ITasksReadService
    {
        private readonly ApplicationContext _context;

        public TasksReadService(ApplicationContext context)
        {
            _context = context;
        }

        public Task<Unit> GetUnit(string userName)
        {
            return _context.Units.AsNoTracking()
                .Include(u => u.Categories)
                    .ThenInclude(c => c.Subcategories)
                        .ThenInclude(s => s.Tasks)
                .Where(unit => unit.UserId == userName)
                .SingleOrDefaultAsync();
        }

        public async Task<Unit> GetUnit(int unitID)
        {
            return await _context.Units.AsNoTracking()
                .Include(u => u.Categories)
                    .ThenInclude(c => c.Subcategories)
                        .ThenInclude(s => s.Tasks)
                .Where(unit => unit.Id == unitID)
                .SingleOrDefaultAsync();
        }

        public Task<Unit> GetUnitBrief(int unitId)
        {
            return _context.Units.AsNoTracking()
                .Where(u => u.Id == unitId)
                .FirstOrDefaultAsync();
        }

        // todo : remove?
        public async Task<Category> GetCategory(int categoryId)
        {
            var category = await _context.Categories.AsNoTracking()
                .Include(c => c.Subcategories)
                    .ThenInclude(s => s.Tasks)
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();

            foreach (var subcategory in category.Subcategories)
            {
                subcategory.Tasks.Select(async task =>
                    task.Subcategories = await GetSubcategoriesByTask(task.Id));
            }
            return category;
        }

        public Task<Category> GetCategoryBrief(int categoryId)
        {
            return _context.Categories.AsNoTracking()
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        public Task<Subcategory> GetSubcategory(int subcategoryId)
        {
            return _context.Subcategories.AsNoTracking()
                .Include(s => s.Tasks)
                .Where(s => s.Id == subcategoryId)
                .FirstOrDefaultAsync();
        }

        public Task<Subcategory> GetSubcategoryBrief(int subcategoryId)
        {
            return _context.Subcategories.AsNoTracking()
                .Where(s => s.Id == subcategoryId)
                .FirstOrDefaultAsync();
        }

        public Task<TaskItem> GetTask(int taskId)
        {
            return _context.Tasks.AsNoTracking()
                .Include(t => t.Subcategories).ThenInclude(s => s.Category)
                .SingleOrDefaultAsync(x => x.Id == taskId);
        }

        public Task<List<Subcategory>> GetSubcategoriesByTask(int taskId)
        {
            return _context.SubcategoriesTasks.AsNoTracking()
               .Include(st => st.Subcategory).ThenInclude(s => s.Category)
               .Include(st => st.TaskItem)
               .Where(st => st.TaskItemId == taskId)
               .Select(st => st.Subcategory)
               .ToListAsync();
        }

        public Task<UserStatistic> GetStatistic(int unitId)
        {
            return _context.Statistics.AsNoTracking()
                .Where(s => s.UnitId == unitId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Tag>> GetTags(int unitId)
        {
            var res = await _context.SubcategoriesTasks.AsNoTracking()
              .Select(st => new Tag
              {
                  CategoryId = st.Subcategory.CategoryId,
                  SubcategoryId = st.SubcategoryId,
                  TaskId = st.TaskItemId,
                  ColorHex = st.Subcategory.Category.ColorHex,
                  TextColorHex = ColorUtil.GenerateTextColorHex(st.Subcategory.Category.ColorHex),
                  Text = st.Subcategory.Name
              })
              .ToListAsync();

            return res;
        }


        //public async Task<TaskItem> GetTasks(int taskId)
        //{
        //    return await _context.Tasks.AsNoTracking()
        //        .Include(t => t.Subcategories).ThenInclude(s => s.Category)
        //        .SingleOrDefaultAsync(x => x.Id == taskId);
        //}

        //public async Task<IEnumerable<TaskItem>> GetConnectedTasks()
        //{
        //    return await _context.Tasks.AsNoTracking()
        //        .Include(t => t.Subcategories)
        //        .Where(t => t.Subcategories.Count > 1)
        //        .ToListAsync();
        //}
    }
}
