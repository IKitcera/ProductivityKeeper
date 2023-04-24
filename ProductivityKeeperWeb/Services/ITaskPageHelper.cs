using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using T = ProductivityKeeperWeb.Models.TaskRelated.TaskItem;

namespace ProductivityKeeperWeb.Services
{
    public interface ITaskPageHelper
    {
        ClaimsIdentity User { get; set; }
        Task<Unit> GetUnit();
        Task<Category> GetCategory(int categoryId);
        Task<Subcategory> GetSubcategory(int categoryId, int subcategoryId);
        Task<T> GetTask(int categoryId, int subcategoryId, int taskId);
        Unit FillUnitForNewcommer(Unit unit);
        Category FillCategory(Category category);
        Subcategory FillSubcategory(int ctgId, Subcategory subcategory);
        T FillTask(int ctgId, int subId, T task);

        Task<TaskToManySubcategories> GetConnectedTaskRelations(int cId, int sId, int tId);
        Task<IEnumerable<T>> GetConnectedTasks(int cId, int sId, int tId);
        System.Threading.Tasks.Task<TaskToManySubcategories> AddConnectedTaskRelation(int[] categories, int[] subcategories, int[] tasks);
        System.Threading.Tasks.Task FullEditOfConnectedTask(int categoryId, int subcategoryId, int taskId, ConnectedToDifferentSubcategoriesTask task);
        System.Threading.Tasks.Task UpdateConnectedTasks(int cId, int sId, int tId, T task);
        System.Threading.Tasks.Task UpdateTask(int categoryId, int subcategoryId, int taskId, T task);
        System.Threading.Tasks.Task DeleteRelatedTasks(int categoryId, int subcategoryId, int taskId);
        System.Threading.Tasks.Task DeleteTask(int categoryId, int subcategoryId, int taskId);
        void ValidateConnectedTaskOnDuplicates(List<TaskToManySubcategories> relations, int tId);
        System.Threading.Tasks.Task ValidateConnectedTaskOnDuplicatesAsync(int tId);
        System.Threading.Tasks.Task ChangeStatus(int categoryId, int subcategoryId, int taskId);
    }
}
