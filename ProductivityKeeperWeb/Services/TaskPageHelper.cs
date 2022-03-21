using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models.TaskRelated;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Services
{
    [Authorize]
    public class TaskPageHelper: ITaskPageHelper
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClaimsIdentity User { get; set; }
        public TaskPageHelper(ApplicationContext _context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = _context;
            _httpContextAccessor = httpContextAccessor;
            User = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
        }

        public async Task<Unit> GetUnit()
        {
            if (!User.IsAuthenticated)
                return null;

            var userId = User.Name;
            var unit = await _context.Units.FirstOrDefaultAsync(u => u.UserId == userId);
            unit.Categories.ForEach(ctg =>
            {
                ctg.Subcategories.ForEach(sub =>
                {
                    sub.Tasks = sub.Tasks.OrderBy(x => x.IsChecked).ToList();
                });
                ctg.Subcategories = ctg.Subcategories.OrderBy(s => s.Position).ToList();
            });

            return unit;
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            var unit = await GetUnit();

            if (unit != null && unit.Categories.Count >= 0)
            {
                var ctg = unit.Categories.FirstOrDefault(cat => cat.Id == categoryId);
                ctg.Subcategories.ForEach(sub =>
                {
                    sub.Tasks = sub.Tasks.OrderBy(x => x.IsChecked).ToList();
                });
                ctg.Subcategories = ctg.Subcategories.OrderBy(s => s.Position).ToList();
                return ctg;
            }
                
            return null;
        }

        public async Task<Subcategory> GetSubcategory(int categoryId, int subcategoryId)
        {
            var ctg = await GetCategory(categoryId);
            var sub = ctg?.Subcategories.FirstOrDefault(s => s.Id == subcategoryId);
            sub.Tasks = sub.Tasks.OrderBy(x => x.IsChecked).ToList();
            return sub;
        }

        public async Task<Models.TaskRelated.Task> GetTask(int categoryId, int subcategoryId, int taskId)
        {
            var sub = await GetSubcategory(categoryId, subcategoryId);
            return sub?.Tasks.FirstOrDefault(t => t.Id == taskId);
        }

        public Category FillCategory(Category category)
        {
            if (category.Color == null)
            {
                System.Random r = new System.Random();
                category.Color = new Color((ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(50, 256));
            }         

            if (string.IsNullOrEmpty(category.Name))
            {
                var unit = GetUnit().Result;
                if (unit == null)
                    return null;

                var lastGeneratedItem = unit.Categories.LastOrDefault(ctg => ctg.Name.Contains(nameof(Category)));
                var lastGeneratedName = lastGeneratedItem != null ? lastGeneratedItem.Name : $"{nameof(Category)} 0";
                string newGeneratedName = nameof(Category) + " " + (int.Parse(lastGeneratedName.Substring((nameof(Category).Length))) + 1).ToString();
                category.Name = newGeneratedName;
            }
            category.DateOfCreation = System.DateTime.Now;
            return category;
        }

        public Subcategory FillSubcategory(int ctgId, Subcategory subcategory)
        {
            if(subcategory.Color == null)
            {
                System.Random r = new System.Random();
                subcategory.Color = new Color((ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(50, 256));
            }

            var ctg = GetCategory(ctgId).Result;

            if (string.IsNullOrEmpty(subcategory.Name))
            {
                var lastGeneratedItem = ctg.Subcategories.LastOrDefault(sub => sub.Name.Contains(nameof(Subcategory)));
                var lastGeneratedName = lastGeneratedItem != null ? lastGeneratedItem.Name : $"{nameof(Subcategory)} 0";
                string newGeneratedName = nameof(Subcategory) + " " + (int.Parse(lastGeneratedName.Substring((nameof(Subcategory).Length))) + 1).ToString();
                subcategory.Name = newGeneratedName;
            }
            subcategory.DateOfCreation = System.DateTime.Now;
            subcategory.Position = ctg.Subcategories.Count;
            return subcategory;
        }

      
        public Models.TaskRelated.Task FillTask(int ctgId, int subId, Models.TaskRelated.Task task)
        {
            if (string.IsNullOrEmpty(task.Text))
                task.Text = "New task";

            task.DateOfCreation = System.DateTime.Now;
            return task;
        }
    }
}
