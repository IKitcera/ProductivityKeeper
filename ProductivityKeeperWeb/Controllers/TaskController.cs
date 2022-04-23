using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T = ProductivityKeeperWeb.Models.TaskRelated.Task;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ITaskPageHelper helper;

        public TaskController(ITaskPageHelper helper, ApplicationContext context)
        {
            _context = context;
            this.helper = helper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> GetTasks(int categoryId, int subcategoryId)
        {
            var sub = await helper.GetSubcategory(categoryId, subcategoryId);
            return sub.Tasks;
        }

        // GET: api/Categorys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> GetTask(int categoryId, int subcategoryId, int taskId)
        {
            var task = await helper.GetTask(categoryId, subcategoryId, taskId);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // PUT: api/Categorys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutTask(int categoryId, int subcategoryId, int taskId, T task)
        {
            if (task.Id != taskId)
            {
                return BadRequest();
            }

            var unit = await helper.GetUnit();

            _context.Entry(unit).State = EntityState.Modified;

            try
            {
                await helper.UpdateTask(categoryId, subcategoryId, taskId, task);
                await helper.UpdateConnectedTasks(categoryId, subcategoryId, taskId, task);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(categoryId, subcategoryId, taskId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPut("edit-connected-task")]
        public async Task<IActionResult> PutTask(int categoryId, int subcategoryId, int taskId, ConnectedToDifferentSubcategoriesTask task)
        {
            try
            {
                var needAddRelations = await helper.GetConnectedTaskRelations(categoryId, subcategoryId, taskId) == null;
                if (needAddRelations)
                {

                    List<T> tasks = new List<T>();
                    for (int i = 0; i < task.CategoriesId.Count; i++)
                    {
                        var s = await helper.GetSubcategory(task.CategoriesId[i], task.SubcategoriesId[i]);
                        var newTask = new T { Text = task.Text, DoneDate = task.DoneDate, IsChecked = task.IsChecked, DateOfCreation = task.DateOfCreation };
                        if (!s.Tasks.Contains(newTask))
                        {
                            s.Tasks.Add(newTask);
                            tasks.Add(newTask);
                        }
                    }
                    await _context.SaveChangesAsync();

                    var taskIds = tasks.Select(x => x.Id);
                    await helper.AddConnectedTaskRelation(categoryId, subcategoryId, taskId, taskIds.ToArray(), task.CategoriesId.ToArray(), task.SubcategoriesId.ToArray());

                    await helper.UpdateTask(categoryId, subcategoryId, taskId, task as T);
                }
                else
                {
                    await PutTask(categoryId, subcategoryId, taskId, task as T);
                }
               
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Categorys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T>> PostTask(int categoryId, int subcategoryId, T task)
        {
            var sub = await helper.GetSubcategory(categoryId, subcategoryId);
            task = helper.FillTask(categoryId, subcategoryId, task);
            sub.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        // DELETE: api/Categorys/5
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int categoryId, int subcategoryId, int taskId)
        {
            await helper.DeleteTask(categoryId, subcategoryId, taskId);
            await helper.DeleteRelatedTasks(categoryId, subcategoryId, taskId);
            return NoContent();
        }

        [HttpPost("/changeStatus")]
        public async Task<IActionResult> ChangeStatus(int categoryId, int subcategoryId, int taskId)
        {
            var task = await helper.GetTask(categoryId, subcategoryId, taskId);
            task.IsChecked = !task.IsChecked;
            task.DoneDate = task.IsChecked ? DateTime.Now : null;
            

            if (task.IsRepeatable && task.IsChecked)
            {
                task.TimesToRepeat = task.TimesToRepeat - 1 > 0 ? task.TimesToRepeat - 1 : 0;
            }

            var connectedTasks = await helper.GetConnectedTasks(categoryId, subcategoryId, taskId);
            foreach (var connectedTask in connectedTasks)
            {
                connectedTask.IsChecked = task.IsChecked;
                connectedTask.DoneDate = task.DoneDate;
                connectedTask.IsRepeatable = task.IsRepeatable;
                connectedTask.TimesToRepeat = task.TimesToRepeat;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("getTaskRelation")]

        public async Task<ActionResult<TaskToManySubcategories>> GetTaskRelations(int categoryId, int subcategoryId, int taskId)
        {
            return await helper.GetConnectedTaskRelations(categoryId, subcategoryId, taskId) ?? new TaskToManySubcategories();
        }

        private bool TaskExists(int categoryId, int subcategoryId, int taskId)
        {
            var sub = helper.GetSubcategory(categoryId, subcategoryId).Result;
            if (sub != null)
                return sub.Tasks.Any(cat => cat.Id == categoryId);

            return false;
        }
    }
}
