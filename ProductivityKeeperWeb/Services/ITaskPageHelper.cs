using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using System.Security.Claims;
using System.Threading.Tasks;
using T = ProductivityKeeperWeb.Models.TaskRelated.Task;

namespace ProductivityKeeperWeb.Services
{
    public interface ITaskPageHelper
    {
        ClaimsIdentity User { get; set; }
        Task<Unit> GetUnit();
        Task<Category> GetCategory(int categoryId);
        Task<Subcategory> GetSubcategory(int categoryId, int subcategoryId);
        Task<T> GetTask(int categoryId, int subcategoryId, int taskId);
        Category FillCategory(Category category);
        Subcategory FillSubcategory(int ctgId, Subcategory subcategory);
        T FillTask(int ctgId, int subId, T task);
    }
}
