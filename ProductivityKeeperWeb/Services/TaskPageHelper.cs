using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using T = ProductivityKeeperWeb.Models.TaskRelated.Task;

namespace ProductivityKeeperWeb.Services
{
    [Authorize]
    public class TaskPageHelper : ITaskPageHelper
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

            if (unit.Categories.Any())
            {
                foreach (var c in unit.Categories)
                {
                    if (!c.Subcategories.Any())
                        continue;

                    foreach (var s in c.Subcategories)
                    {
                        if (s.Tasks.Any())
                            s.Tasks = s.Tasks.OrderBy(x => x.IsChecked).ToList();
                    }

                    c.Subcategories = c.Subcategories.OrderBy(s => s.Position).ToList();
                }
                unit.Categories = unit.Categories.OrderBy(c => c.Position).ToList();
            }
         //    await _context.SaveChangesAsync();

            return unit;
        }

        public async Task<Category> GetCategory(int categoryId)
        {
            var unit = await GetUnit();

            if (unit != null && unit.Categories.Count >= 0)
            {
                var ctg = unit.Categories.FirstOrDefault(cat => cat.Id == categoryId);

                if (ctg == null) return null;

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
            if (sub == null) return null;
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
            category.IsVisible = true;

            category.DateOfCreation = System.DateTime.Now;
            return category;
        }

        public Subcategory FillSubcategory(int ctgId, Subcategory subcategory)
        {
            if (subcategory.Color == null)
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

        public async Task<TaskToManySubcategories> GetConnectedTaskRelations(int cId, int sId, int tId)
        {
            var unit = await GetUnit();
            return unit.TaskToManySubcategories.FirstOrDefault(x => x.TaskSubcategories
                .Any(t => t.TaskId == tId));
        }

        public async Task<IEnumerable<Models.TaskRelated.Task>> GetConnectedTasks(int cId, int sId, int tId)
        {
            IEnumerable<Models.TaskRelated.Task> tasks = new List<Models.TaskRelated.Task>();

            var relation = await GetConnectedTaskRelations(cId, sId, tId);

            if (relation != null)
            {
                foreach (var x in relation.TaskSubcategories)
                {
                    var task = await GetTask(x.CategoryId, x.SubcategoryId, x.TaskId);
                    if(task != null)
                        (tasks as List<Models.TaskRelated.Task>).Add(task);
                }
            }
            return tasks;
        }

        public void ValidateConnectedTaskOnDuplicates(List<TaskToManySubcategories> relations, int tId)
        {
            relations.ForEach(x =>
            {
                if (x.TaskSubcategories.Any(t => t.TaskId == tId))
                    throw new System.Exception("This task is already connected");
            });
        }

        public async System.Threading.Tasks.Task ValidateConnectedTaskOnDuplicatesAsync(int tId)
        {
            var taskRelationExisted = (await GetUnit()).TaskToManySubcategories;
            ValidateConnectedTaskOnDuplicates(taskRelationExisted, tId);
        }

        public async System.Threading.Tasks.Task<TaskToManySubcategories> AddConnectedTaskRelation(int[] categories, int[] subcategories, int[] tasks)
        {
            var taskRelationExisted = (await GetUnit()).TaskToManySubcategories;
            var existedTaskSub = taskRelationExisted.FirstOrDefault(t => t.TaskSubcategories.Any(t => tasks.Any(id => id == t.TaskId)));

            TaskToManySubcategories taskRelation = new TaskToManySubcategories();

            for (int i = 0; i < tasks.Length; i++)
            {
                taskRelation.TaskSubcategories.Add(
                    new TaskSubcategory
                    {
                        CategoryId = categories[i],
                        SubcategoryId = subcategories[i],
                        TaskId = tasks[i]
                    });
            }


            if (existedTaskSub == null)
                taskRelationExisted.Add(taskRelation);
            else
                existedTaskSub.TaskSubcategories = taskRelation.TaskSubcategories;

            await _context.SaveChangesAsync();

            var unit = await GetUnit();
            var relationAfterSave = unit.TaskToManySubcategories.Find(t => t.TaskSubcategories == taskRelation.TaskSubcategories);
            return relationAfterSave;
        }

        public async System.Threading.Tasks.Task FullEditOfConnectedTask(int categoryId, int subcategoryId, int taskId, ConnectedToDifferentSubcategoriesTask task)
        {
            List<Models.TaskRelated.Task> tasks = new List<Models.TaskRelated.Task>();
            List<int> notAddedIds = new List<int>();

            var ctgIds = new List<int>();
            var subIds = new List<int>();

            var connections = await GetConnectedTaskRelations(categoryId, subcategoryId, taskId);

            // Clear duplicates
            for (int i = 0; i < task.CategoriesId.Count; i++)
            {
                var x = new { c = task.CategoriesId[i], s = task.SubcategoriesId[i] };
                bool breaked = false;
                for (int j = 0; j < ctgIds.Count; j++)
                {
                    var y = new { c = ctgIds[j], s = subIds[j] };

                    if (x.c == y.c && x.s == y.s)
                    {
                        breaked = true;
                        break;
                    }
                }

                if (!breaked && (x.c != categoryId || x.s != subcategoryId))
                {
                    ctgIds.Add(task.CategoriesId[i]);
                    subIds.Add(task.SubcategoriesId[i]);
                }
            }

            for (int i = 0; i < ctgIds.Count; i++)
            {
                var s = await GetSubcategory(ctgIds[i], subIds[i]);

                if (subcategoryId != s.Id &&
                    (connections == null || connections.TaskSubcategories.Any(ts => ts.SubcategoryId != s.Id)))
                {
                    var newTask = new Models.TaskRelated.Task
                    {
                        Text = task.Text,
                        DoneDate = task.DoneDate,
                        IsChecked = task.IsChecked,
                        DateOfCreation = task.DateOfCreation,
                        Deadline = task.Deadline,
                        IsRepeatable = task.IsRepeatable,
                        GoalRepeatCount = task.GoalRepeatCount,
                        TimesToRepeat = task.TimesToRepeat,
                        HabbitIntervalInHours = task.HabbitIntervalInHours
                    };

                    s.Tasks.Add(newTask);
                    tasks.Add(newTask);
                }
            }

            await _context.SaveChangesAsync();


            tasks.Add(task);
            ctgIds.Add(categoryId);
            subIds.Add(subcategoryId);

            var taskIds = tasks.Select(x => x.Id).ToList();



            await DeleteUnusedConnectedRelation(connections, taskIds);

            var newConnections = await AddConnectedTaskRelation(ctgIds.ToArray(), subIds.ToArray(), taskIds.ToArray());

            UpdateTasksWithRelationReference(newConnections, tasks);

            await UpdateTask(categoryId, subcategoryId, taskId, task as Models.TaskRelated.Task);
        }

        public async System.Threading.Tasks.Task UpdateConnectedTasks(int cId, int sId, int tId, Models.TaskRelated.Task task)
        {
            var tasks = await GetConnectedTaskRelations(cId, sId, tId);
            if (tasks == null)
                return;

            foreach (var relation in tasks.TaskSubcategories)
            {
                await UpdateTask(relation.CategoryId, relation.SubcategoryId, relation.TaskId, task);
            }
        }

        public async System.Threading.Tasks.Task UpdateTask(int categoryId, int subcategoryId, int taskId, Models.TaskRelated.Task task)
        {
            var unit = await GetUnit();

            await EnterModifyState(unit);

            var sub = unit.Categories
                    .Where(c => c.Id == categoryId).First().Subcategories
                    .Where(s => s.Id == subcategoryId).First();
            var tsk = sub.Tasks
                 .Where(t => t.Id == taskId)?.First();



            tsk.Text = task.Text;
            tsk.Deadline = task.Deadline?.ToLocalTime();
            tsk.DoneDate = task.IsChecked ? DateTime.Now : null;
            tsk.IsChecked = task.IsChecked;

            tsk.IsRepeatable = task.IsRepeatable;

            if (task.IsRepeatable)
            {
                var completed = tsk.GoalRepeatCount - tsk.TimesToRepeat;

                tsk.GoalRepeatCount = task.GoalRepeatCount;
                tsk.TimesToRepeat = completed > 0 ? task.GoalRepeatCount - completed : task.GoalRepeatCount;
                tsk.HabbitIntervalInHours = task.HabbitIntervalInHours;
            }
            else
            {
                tsk.GoalRepeatCount = null;
                tsk.TimesToRepeat = null;
                tsk.HabbitIntervalInHours = null;
            }

            tsk.RelationId = task.RelationId;

            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteRelatedTasks(int categoryId, int subcategoryId, int taskId)
        {
            var tasks = await GetConnectedTaskRelations(categoryId, subcategoryId, taskId);
            if (tasks == null)
                return;

            var unit = await GetUnit();

            foreach (var relation in tasks.TaskSubcategories)
            {
                await DeleteTask(relation.CategoryId, relation.SubcategoryId, relation.TaskId);
            }

            await EnterModifyState(unit);
            unit.TaskToManySubcategories.Remove(tasks);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteTask(int categoryId, int subcategoryId, int taskId)
        {
            await EnterModifyState();

            var sub = await GetSubcategory(categoryId, subcategoryId);
            var task = sub.Tasks.FirstOrDefault(t => t.Id == taskId);

            if (sub == null)
            {
                throw new Exception("Couldn't found such subcategory");
            }

            AddDeletingTaskToTheArchive(task);

            sub.Tasks.Remove(task);

            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task ChangeStatus(int categoryId, int subcategoryId, int taskId)
        {
            await EnterModifyState();
            var task = await GetTask(categoryId, subcategoryId, taskId);
            task.IsChecked = !task.IsChecked;
            task.DoneDate = task.IsChecked ? DateTime.Now : null;


            if (task.IsRepeatable && task.IsChecked)
            {
                task.TimesToRepeat = task.TimesToRepeat - 1 > 0 ? task.TimesToRepeat - 1 : 0;
            }

            var connectedTasks = await GetConnectedTasks(categoryId, subcategoryId, taskId);
            foreach (var connectedTask in connectedTasks)
            {
                connectedTask.IsChecked = task.IsChecked;
                connectedTask.DoneDate = task.DoneDate;
                connectedTask.IsRepeatable = task.IsRepeatable;
                connectedTask.TimesToRepeat = task.TimesToRepeat;
            }
            await _context.SaveChangesAsync();
        }

        private void AddDeletingTaskToTheArchive(Models.TaskRelated.Task task)
        {
            if (task == null)
                return;

            var unit = System.Threading.Tasks.Task.Run(async () => await GetUnit()).Result;
            var archievedTask = new ArchivedTask();
            if (task.IsChecked)
            {
                archievedTask.Status = ArchievedTaskStatus.Done;
                archievedTask.DoneDate = task.DoneDate;
            }
            else if (task.Deadline.HasValue && DateTime.Now.Date > task.Deadline.Value)
            {
                archievedTask.Status = ArchievedTaskStatus.Expired;
            }
            else
            {
                archievedTask.Status = ArchievedTaskStatus.Undone;
            }
            archievedTask.Deadline = task.Deadline;

            unit.TaskArchive.Add(archievedTask);
        }

        private async System.Threading.Tasks.Task EnterModifyState(Unit unit = null)
        {
            unit ??= await GetUnit();
            _context.Entry(unit).State = EntityState.Modified;
        }

        private async System.Threading.Tasks.Task DeleteUnusedConnectedRelation(TaskToManySubcategories connections, IEnumerable<int> taskIds)
        {
            if (connections != null)
            {
                foreach (var c in connections.TaskSubcategories.Where(ts => !taskIds.Contains(ts.TaskId)))
                {
                    var sub = await GetSubcategory(c.CategoryId, c.SubcategoryId);
                    var t = sub.Tasks.FirstOrDefault(tt => tt.Id == c.TaskId);
                    sub.Tasks.Remove(t);
                }
            }


            await _context.SaveChangesAsync();
        }

        private void UpdateTasksWithRelationReference(TaskToManySubcategories newConnections, IEnumerable<T> tasks)
        {
            if (newConnections != null)
            {
                foreach (var t in tasks)
                {
                    t.RelationId = newConnections.Id;
                }
            }
        }
    }

}
