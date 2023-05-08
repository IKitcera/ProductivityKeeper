﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Repositories.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<Unit> GetUnit(string userName)
        {
            return await _context.Units.AsNoTracking()
                .Include(u => u.Categories)
                    .ThenInclude(c => c.Subcategories)
                        .ThenInclude(s => s.Tasks)
                .Where(unit => unit.UserId == userName)
                .SingleOrDefaultAsync();
        }

        public async Task<Unit> GetUnit(int unitID)
        {
            return await _context.Units.AsNoTracking()
                .Include(u => u.Categories)
                    .ThenInclude(c => c.Subcategories)
                        .ThenInclude(s => s.Tasks)
                .Where(unit => unit.Id == unitID)
                .SingleOrDefaultAsync();
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

        public Task<UserStatistic> GetStatistic(int unitId)
        {
            return _context.Statistics.AsNoTracking()
                .Where(s => s.UnitId == unitId)
                .FirstOrDefaultAsync();
        }

        //public async Task<IEnumerable<TaskItem>> GetTasks()
        //{
        //    return await _context.Tasks.AsNoTracking()
        //        .Include(t => t.Subcategories)
        //        .ToListAsync();
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
