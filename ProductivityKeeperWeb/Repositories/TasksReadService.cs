using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Repositories
{
    [Authorize]
    public class TasksReadService : ITasksReadService
    {
        private readonly ApplicationContext _context;

        public TasksReadService(ApplicationContext context)
        {
            _context = context;
        }

        public Task<Unit> GetUnit(int unitId)
        {
            return _context.Units.AsNoTracking()
                .Include(u => u.Categories)
                    .ThenInclude(c => c.Subcategories)
                        .ThenInclude(s => s.Tasks)
                .Where(u => u.Id == unitId)
                .FirstOrDefaultAsync();
        }

        public Task<Unit> GetUnitBrief(int unitId)
        {
            return _context.Units.AsNoTracking()
                .Where(u => u.Id == unitId)
                .FirstOrDefaultAsync();
        }

        // todo : remove?
        public Task<Category> GetCategory(int categoryId)
        {
            return _context.Categories.AsNoTracking()
                .Include(c => c.Subcategories)
                    .ThenInclude(s => s.Tasks)
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
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
                .Include(t => t.Subcategories)
                .Where(t => t.Id == taskId)
                .FirstOrDefaultAsync();
        }
    }
}
