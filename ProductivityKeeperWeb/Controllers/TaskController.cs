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
using T = ProductivityKeeperWeb.Models.TaskRelated.TaskItem;

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
                // await helper.UpdateConnectedTasks(categoryId, subcategoryId, taskId, task);
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
                await helper.FullEditOfConnectedTask(categoryId, subcategoryId, taskId, task);
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

            if (sub.Name.ToLower() == "today")
            {
                task.Deadline ??= DateTime.Now.Date.AddDays(1).Subtract(new TimeSpan(0, 0, 1));
            }

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
            await helper.ChangeStatus(categoryId, subcategoryId, taskId);
            return Ok();
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
