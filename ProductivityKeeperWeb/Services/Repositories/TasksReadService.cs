using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using ProductivityKeeperWeb.Domain.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Services.Repositories
{
    [Authorize]
    public class TasksReadService : ITasksReadService
    {
        private readonly ApplicationContext _context;
        private readonly IAuthService _authService;

        public TasksReadService(ApplicationContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Unit> GetUnit()
        {
            var unit = await _context.Units.AsNoTracking()
                .Include(u => u.Categories)
                    .ThenInclude(c => c.Subcategories)
                        .ThenInclude(s => s.Tasks)
                .FirstOrDefaultAsync(unit => unit.Id == _authService.GetUnitId());


            unit.Categories = unit.Categories.OrderBy(c => c.Position).ToList();

            foreach (var ctg in unit.Categories.Where(c => c.Subcategories.Count > 0))
            { 
                ctg.Subcategories = ctg.Subcategories.OrderBy(s => s.Position).ToList();

                foreach(var sub in ctg.Subcategories.Where(s => s.Tasks.Count > 0))
                {
                    sub.Tasks = sub.Tasks.OrderBy(x => x.IsChecked).ThenBy(x => x.Position).ToList();
                }
            }
            return unit;
        }

        public Task<Unit> GetUnitBrief()
        {
            return _context.Units.AsNoTracking()
                .Where(u => u.Id == _authService.GetUnitId())
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

        public Task<UserStatistic> GetStatistic()
        {
            return _context.Statistics.AsNoTracking()
                .Where(s => s.UnitId == _authService.GetUnitId())
                .FirstOrDefaultAsync();
        }

        public async Task<List<Tag>> GetTags()
        {
            var res = await _context.SubcategoriesTasks.AsNoTracking()
              .Where(st => st.Subcategory.Category.UnitId == _authService.GetUnitId())
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
