using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Repositories.Interfaces;
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


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Subcategory>>> GetSubcategories(int categoryId)
        {
            var ctg = await _taskReadService.GetCategory(categoryId);
            return ctg.Subcategories;
        }

        // GET: api/Categorys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategory>> GetSubategory(int subcategoryId)
        {
            var sub = await _taskReadService.GetSubcategory(subcategoryId);
            if (sub == null)
            {
                return NotFound();
            }

            return sub;
        }

        // PUT: api/Categorys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubcategory(int subcategoryId, Subcategory subcategory)
        {
            if (subcategoryId != subcategory.Id)
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

        // DELETE: api/Categorys/5
        [HttpDelete]
        public async Task<IActionResult> DeleteSubcategory(int subcategoryId)
        {
            await _taskWriteService.DeleteSubcategory(subcategoryId);
            return NoContent();
        }
    }
}
