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
    public class SubcategoryController : ControllerBase
    {
        private readonly ITasksReadService _taskReadService;
        private readonly ITasksWriteService _taskWriteService;
        public SubcategoryController(
            ITasksReadService taskReadService,
            ITasksWriteService taskWriteService
            )
        {
            _taskReadService = taskReadService;
            _taskWriteService = taskWriteService;
        }

        // PUT: api/Categorys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubcategory(int id, Subcategory subcategory)
        {
            if (id != subcategory.Id)
            {
                return BadRequest();
            }

            try
            {
                var res = await _taskWriteService.UpdateSubcategory(subcategory);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // POST: api/Categorys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subcategory>> PostSubcategory(Subcategory subcategory)
        {
            return await _taskWriteService.AddSubcategory(subcategory);
        }

        [HttpPost("reorder")]
        public async Task ReorderSubs([FromBody] IEnumerable<int> ids)
        {
            await _taskWriteService.ReorderSubcategories(ids);
        }

        // DELETE: api/Subcategory/5
        [HttpDelete("{subcategoryId}")]
        public async Task<IActionResult> DeleteSubcategory(int subcategoryId)
        {
            await _taskWriteService.DeleteSubcategory(subcategoryId);
            return NoContent();
        }
    }
}
