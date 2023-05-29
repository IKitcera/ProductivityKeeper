﻿using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface ITasksReadService
    {
        Task<Unit> GetUnit(string userName);
        Task<Unit> GetUnit(int unitID);
        Task<Unit> GetUnitBrief(int unitId);

        Task<Category> GetCategory(int categoryId);
        Task<Category> GetCategoryBrief(int categoryId);

        Task<Subcategory> GetSubcategory(int subcategoryId);
        Task<Subcategory> GetSubcategoryBrief(int subcategoryId);

        Task<TaskItem> GetTask(int taskId);
        Task<List<Tag>> GetTags(int unitId);

        //  Task<IEnumerable<TaskItem>> GetTasks(int taskId);

        Task<UserStatistic> GetStatistic(int unitId);
    }
}