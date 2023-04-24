using ProductivityKeeperWeb.Models.TaskRelated;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Repositories.Interfaces
{
    public interface ITasksReadService
    {
        Task<Unit> GetUnit(int unitId);
        Task<Unit> GetUnitBrief(int unitId);

        Task<Category> GetCategory(int categoryId);
        Task<Category> GetCategoryBrief(int categoryId);

        Task<Subcategory> GetSubcategory(int subcategoryId);
        Task<Subcategory> GetSubcategoryBrief(int subcategoryId);

        Task<TaskItem> GetTask(int taskId);
    }
}
