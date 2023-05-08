using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITasksReadService _taskReadService;
        private readonly ITasksWriteService _taskWriteService;

        public TaskController(
            ITasksReadService taskReadService,
            ITasksWriteService taskWriteService)
        {
            _taskReadService = taskReadService;
            _taskWriteService = taskWriteService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks(int subcategoryId)
        {
            Subcategory sub = await _taskReadService.GetSubcategory(subcategoryId);
            return sub.Tasks;
        }

        // GET: api/Categorys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int taskId)
        {
            TaskItem task = await _taskReadService.GetTask(taskId);
            return task == null ? (ActionResult<TaskItem>)NotFound() : (ActionResult<TaskItem>)task;
        }

        // PUT: api/Categorys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<TaskItem>> PutTask(int taskId, TaskItem task)
        {
            if (task.Id != taskId)
            {
                return BadRequest();
            }

            try
            {
                return await _taskWriteService.UpdateTaskItem(task);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        // POST: api/Categorys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTask(TaskItem task)
        {
            return await _taskWriteService.AddTaskItem(task);
        }

        // DELETE: api/Categorys/5
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int categoryId, int subcategoryId, int taskId)
        {
            await _taskWriteService.DeleteTaskItem(taskId);
            return Ok();
        }

        //[HttpPost("/changeStatus")]
        //public async Task<IActionResult> ChangeStatus(int categoryId, int subcategoryId, int taskId)
        //{
        //    await helper.ChangeStatus(categoryId, subcategoryId, taskId);
        //    return Ok();
        //}
    }
}
