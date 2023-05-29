using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface ITasksWriteService
    {
        // Category 
        Task<Category> AddCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task DeleteCategory(int categoryId);

        // Subcategory 
        Task<Subcategory> AddSubcategory(Subcategory subcategory);
        Task<Subcategory> UpdateSubcategory(Subcategory subcategory);
        Task ReorderSubcategories(IEnumerable<int> ids);
        Task DeleteSubcategory(int subcategoryId);

        // TaskItem
        Task<TaskItem> AddTaskItem(TaskItem task);
        Task<TaskItem> UpdateTaskItem(TaskItem task);
        Task<TaskItem> ChangeTaskStatus(int taskId);
        Task DeleteTaskItem(int taskId, int unitId);

        // Statistic
        Task<UserStatistic> UpdateUserStatistic(UserStatistic statistic);

        Task<Unit> AddUnitForNewCommer(string email);
    }
}
