using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface ITasksReadService
    {
        Task<Unit> GetUnit(int? unitId = null);
        Task<Unit> GetUnitBrief();

        Task<Category> GetCategory(int categoryId);
        Task<Category> GetCategoryBrief(int categoryId);

        Task<Subcategory> GetSubcategory(int subcategoryId);
        Task<Subcategory> GetSubcategoryBrief(int subcategoryId);

        Task<TaskItem> GetTask(int taskId);
        Task<List<Tag>> GetTags();

        //  Task<IEnumerable<TaskItem>> GetTasks(int taskId);

        Task<UserStatistic> GetStatistic();
    }
}
