using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
                var tsk = unit.Categories
                     .Where(c => c.Id == categoryId).First().Subcategories
                     .Where(s => s.Id == subcategoryId).First().Tasks
                     .Where(t => t.Id == taskId).First();


                tsk.IsChecked = task.IsChecked;

                if (tsk.IsChecked)
                    tsk.DoneDate = DateTime.Now;

                tsk.Deadline = task.Deadline;

                await _context.SaveChangesAsync();
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int categoryId, int subcategoryId, int taskId)
        {
            var sub = await helper.GetSubcategory(categoryId, subcategoryId);
            var task = sub.Tasks.FirstOrDefault(t => t.Id == taskId);

            if (sub == null)
            {
                return NotFound();
            }

            sub.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("/changeStatus")]
        public async Task<IActionResult> ChangeStatus(int categoryId, int subcategoryId, int taskId)
        {
            var task = await helper.GetTask(categoryId, subcategoryId, taskId);
            task.IsChecked = !task.IsChecked;
            await _context.SaveChangesAsync();
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
