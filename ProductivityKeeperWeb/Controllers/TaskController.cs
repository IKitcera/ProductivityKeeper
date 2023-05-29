using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
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
        private readonly IAuthService _authService;

        public TaskController(
            ITasksReadService taskReadService,
            ITasksWriteService taskWriteService,
            IAuthService authService)
        {
            _taskReadService = taskReadService;
            _taskWriteService = taskWriteService;
            _authService = authService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks(int subcategoryId)
        {
            Subcategory sub = await _taskReadService.GetSubcategory(subcategoryId);
            return sub.Tasks;
        }

        // GET: api/Categorys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            TaskItem task = await _taskReadService.GetTask(id);
            return task == null ? (ActionResult<TaskItem>)NotFound() : (ActionResult<TaskItem>)task;
        }

        [HttpGet("tags")]
        public async Task<ActionResult<List<Tag>>> GetTags()
        {
            var res = await _taskReadService.GetTags();
            return res;
        }

        // PUT: api/Categorys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItem>> PutTask(int id, TaskItem task)
        {
            if (task.Id != id)
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

        [HttpPost("change-status")]
        public async Task<ActionResult<TaskItem>> ChangeTasksStatus([FromBody]int taskId)
        {
            return await _taskWriteService.ChangeTaskStatus(taskId);
        }

        // POST: api/Categorys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTask(TaskItem task)
        {
            return await _taskWriteService.AddTaskItem(task);
        }

        // DELETE: api/Categorys/5
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            await _taskWriteService.DeleteTaskItem(taskId, _authService.GetUnitId());
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
