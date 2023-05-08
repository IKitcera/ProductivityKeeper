//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using ProductivityKeeperWeb.BussinessLogicLayer.Utils;
//using ProductivityKeeperWeb.Data;
//using ProductivityKeeperWeb.Models.TaskRelated;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using T = ProductivityKeeperWeb.Models.TaskRelated.TaskItem;
//using TaskItem = ProductivityKeeperWeb.Models.TaskRelated.TaskItem;

//namespace ProductivityKeeperWeb.Services
//{
//    [Authorize]
//    public class TaskPageHelper : ITaskPageHelper
//    {
//        private readonly ApplicationContext _context;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        public ClaimsIdentity User { get; set; }
//        public TaskPageHelper(ApplicationContext _context, IHttpContextAccessor httpContextAccessor)
//        {
//            this._context = _context;
//            User = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
//        }

       
//        public async System.Threading.Tasks.Task FullEditOfConnectedTask(int categoryId, int subcategoryId, int taskId, ConnectedToDifferentSubcategoriesTask task)
//        {
//            List<Models.TaskRelated.TaskItem> tasks = new List<Models.TaskRelated.TaskItem>();
//            List<int> notAddedIds = new List<int>();

//            var ctgIds = new List<int>();
//            var subIds = new List<int>();

//            var connections = await GetConnectedTaskRelations(categoryId, subcategoryId, taskId);

//            // Clear duplicates
//            for (int i = 0; i < task.CategoriesId.Count; i++)
//            {
//                var x = new { c = task.CategoriesId[i], s = task.SubcategoriesId[i] };
//                bool breaked = false;
//                for (int j = 0; j < ctgIds.Count; j++)
//                {
//                    var y = new { c = ctgIds[j], s = subIds[j] };

//                    if (x.c == y.c && x.s == y.s)
//                    {
//                        breaked = true;
//                        break;
//                    }
//                }

//                if (!breaked && (x.c != categoryId || x.s != subcategoryId))
//                {
//                    ctgIds.Add(task.CategoriesId[i]);
//                    subIds.Add(task.SubcategoriesId[i]);
//                }
//            }

//            for (int i = 0; i < ctgIds.Count; i++)
//            {
//                var s = await GetSubcategory(ctgIds[i], subIds[i]);

//                if (subcategoryId != s.Id &&
//                    (connections == null || connections.TaskSubcategories.Any(ts => ts.SubcategoryId != s.Id)))
//                {
//                    var newTask = new Models.TaskRelated.TaskItem
//                    {
//                        Text = task.Text,
//                        DoneDate = task.DoneDate,
//                        IsChecked = task.IsChecked,
//                        DateOfCreation = task.DateOfCreation,
//                        Deadline = task.Deadline,
//                        IsRepeatable = task.IsRepeatable,
//                        GoalRepeatCount = task.GoalRepeatCount,
//                        TimesToRepeat = task.TimesToRepeat,
//                        HabbitIntervalInHours = task.HabbitIntervalInHours
//                    };

//                    s.Tasks.Add(newTask);
//                    tasks.Add(newTask);
//                }
//            }

//            await this.EnterModifyState();
//            await _context.SaveChangesAsync();


//            tasks.Add(task);
//            ctgIds.Add(categoryId);
//            subIds.Add(subcategoryId);

//            var taskIds = tasks.Select(x => x.Id).ToList();



//            await DeleteUnusedConnectedRelation(connections, taskIds);

//            var newConnections = await AddConnectedTaskRelation(ctgIds.ToArray(), subIds.ToArray(), taskIds.ToArray());

//            UpdateTasksWithRelationReference(newConnections, tasks);

//            await UpdateTask(categoryId, subcategoryId, taskId, task as Models.TaskRelated.TaskItem);
//        }

//        public async System.Threading.Tasks.Task UpdateConnectedTasks(int cId, int sId, int tId, Models.TaskRelated.TaskItem task)
//        {
//            var tasks = await GetConnectedTaskRelations(cId, sId, tId);
//            if (tasks == null)
//                return;

//            foreach (var relation in tasks.TaskSubcategories)
//            {
//                await UpdateTask(relation.CategoryId, relation.SubcategoryId, relation.TaskId, task);
//            }
//        }

//        public async System.Threading.Tasks.Task UpdateTask(int categoryId, int subcategoryId, int taskId, Models.TaskRelated.TaskItem task)
//        {
//            var unit = await GetUnit();


//            var sub = unit.Categories
//                    .Where(c => c.Id == categoryId).First().Subcategories
//                    .Where(s => s.Id == subcategoryId).First();
//            var tsk = sub.Tasks
//                 .Where(t => t.Id == taskId)?.First();



//            tsk.Text = task.Text;
//            tsk.Deadline = task.Deadline?.ToLocalTime();
//            tsk.DoneDate = task.IsChecked ? DateTime.Now : null;
//            tsk.IsChecked = task.IsChecked;

//            tsk.IsRepeatable = task.IsRepeatable;

//            if (task.IsRepeatable)
//            {
//                var completed = tsk.GoalRepeatCount - tsk.TimesToRepeat;

//                tsk.GoalRepeatCount = task.GoalRepeatCount;
//                tsk.TimesToRepeat = completed > 0 ? task.GoalRepeatCount - completed : task.GoalRepeatCount;
//                tsk.HabbitIntervalInHours = task.HabbitIntervalInHours;
//            }
//            else
//            {
//                tsk.GoalRepeatCount = null;
//                tsk.TimesToRepeat = null;
//                tsk.HabbitIntervalInHours = null;
//            }

//            tsk.RelationId = task.RelationId;

//            await EnterModifyState(unit);
//            await _context.SaveChangesAsync();
//        }

//        public async System.Threading.Tasks.Task DeleteRelatedTasks(int categoryId, int subcategoryId, int taskId)
//        {
//            var tasks = await GetConnectedTaskRelations(categoryId, subcategoryId, taskId);
//            if (tasks == null)
//                return;

//            var unit = await GetUnit();

//            foreach (var relation in tasks.TaskSubcategories)
//            {
//                await DeleteTask(relation.CategoryId, relation.SubcategoryId, relation.TaskId);
//            }

//            await EnterModifyState(unit);
//            unit.TaskToManySubcategories.Remove(tasks);
//            await _context.SaveChangesAsync();
//        }

//        public async System.Threading.Tasks.Task DeleteTask(int categoryId, int subcategoryId, int taskId)
//        {
//            await EnterModifyState();

//            var sub = await GetSubcategory(categoryId, subcategoryId);
//            var task = sub.Tasks.FirstOrDefault(t => t.Id == taskId);

//            if (sub == null)
//            {
//                throw new Exception("Couldn't found such subcategory");
//            }

//            AddDeletingTaskToTheArchive(task);

//            sub.Tasks.Remove(task);

//            await _context.SaveChangesAsync();
//        }

//        public async System.Threading.Tasks.Task ChangeStatus(int categoryId, int subcategoryId, int taskId)
//        {
//            await EnterModifyState();
//            var task = await GetTask(categoryId, subcategoryId, taskId);
//            task.IsChecked = !task.IsChecked;
//            task.DoneDate = task.IsChecked ? DateTime.Now : null;


//            if (task.IsRepeatable && task.IsChecked)
//            {
//                task.TimesToRepeat = task.TimesToRepeat - 1 > 0 ? task.TimesToRepeat - 1 : 0;
//            }

//            var connectedTasks = await GetConnectedTasks(categoryId, subcategoryId, taskId);
//            foreach (var connectedTask in connectedTasks)
//            {
//                connectedTask.IsChecked = task.IsChecked;
//                connectedTask.DoneDate = task.DoneDate;
//                connectedTask.IsRepeatable = task.IsRepeatable;
//                connectedTask.TimesToRepeat = task.TimesToRepeat;
//            }
//            await _context.SaveChangesAsync();
//        }

//        private void AddDeletingTaskToTheArchive(Models.TaskRelated.TaskItem task)
//        {
//            if (task == null)
//                return;

//            var unit = System.Threading.Tasks.Task.Run(async () => await GetUnit()).Result;
//            var archievedTask = new ArchivedTask();
//            if (task.IsChecked)
//            {
//                archievedTask.Status = ArchievedTaskStatus.Done;
//                archievedTask.DoneDate = task.DoneDate;
//            }
//            else if (task.Deadline.HasValue && DateTime.Now.Date > task.Deadline.Value)
//            {
//                archievedTask.Status = ArchievedTaskStatus.Expired;
//            }
//            else
//            {
//                archievedTask.Status = ArchievedTaskStatus.Undone;
//            }
//            archievedTask.Deadline = task.Deadline;

//            unit.TaskArchive.Add(archievedTask);
//        }

//        private async System.Threading.Tasks.Task EnterModifyState(Unit unit = null)
//        {
//            unit ??= await GetUnit();
//            _context.Entry(unit).State = EntityState.Modified;
//        }

//        private async System.Threading.Tasks.Task DeleteUnusedConnectedRelation(TaskToManySubcategories connections, IEnumerable<int> taskIds)
//        {
//            if (connections != null)
//            {
//                var unusedConnections = connections.TaskSubcategories.Where(ts => !taskIds.Contains(ts.TaskId));

//                foreach (var c in unusedConnections)
//                {
//                    var sub = await GetSubcategory(c.CategoryId, c.SubcategoryId);
//                    var t = sub.Tasks.FirstOrDefault(tt => tt.Id == c.TaskId);
//                    sub.Tasks.Remove(t);
//                }
//            }

//            await EnterModifyState();
//            await _context.SaveChangesAsync();
//        }

//        private void UpdateTasksWithRelationReference(TaskToManySubcategories newConnections, IEnumerable<T> tasks)
//        {
//            if (newConnections != null)
//            {
//                foreach (var t in tasks)
//                {
//                    t.RelationId = newConnections.Id;
//                }
//            }
//        }



//        //---------------------

       
//    }

//}
