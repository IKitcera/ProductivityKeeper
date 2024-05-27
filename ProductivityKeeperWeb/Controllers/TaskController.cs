using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System.Collections.Generic;
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
            ITasksWriteService taskWriteService
            )
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
        public async Task<ActionResult<TaskItem>> ChangeTasksStatus([FromBody] int taskId)
        {
            return await _taskWriteService.ChangeTaskStatus(taskId);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTask(TaskItem task)
        {
            return await _taskWriteService.AddTaskItem(task);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            await _taskWriteService.DeleteTaskItem(taskId);
            return Ok();
        }

        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderTasks([FromBody] IEnumerable<int> ids)
        {
            await _taskWriteService.ReorderTasks(ids);
            return Ok();
        }
    }
}
