using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
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
        Task DeleteSubcategory(int subcategoryId);

        // TaskItem
        Task<TaskItem> AddTaskItem(TaskItem task);
        Task<TaskItem> UpdateTaskItem(TaskItem task);
        Task DeleteTaskItem(int taskId);

        // Statistic
        Task<UserStatistic> UpdateUserStatistic(UserStatistic statistic);
    }
}
