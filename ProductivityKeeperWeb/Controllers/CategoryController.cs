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
    public class CategoryController : ControllerBase
    {
        private readonly ITasksReadService _taskReadService;
        private readonly ITasksWriteService _taskWriteService;


        public CategoryController(
            ITasksReadService taskReadService,
            ITasksWriteService taskWriteService)
        {
            _taskReadService = taskReadService;
            _taskWriteService = taskWriteService;
        }


        [HttpGet]
        public async Task<ActionResult<Unit>> GetUnit()
        {
            Unit unit = await _taskReadService.GetUnit(User.Identity.Name);
            return unit == null ? (ActionResult<Unit>)BadRequest("Invalid unit id") : (ActionResult<Unit>)Ok(unit);
        }

        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetJustCategories(int unitId)
        {
            Unit unit = await _taskReadService.GetUnit(User.Identity.Name);
            return unit == null ? (ActionResult<IEnumerable<Category>>)Unauthorized() : (ActionResult<IEnumerable<Category>>)unit.Categories;
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            Category category = await _taskReadService.GetCategory(id);

            return category == null ? (ActionResult<Category>)NotFound() : (ActionResult<Category>)category;
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            Category res = await _taskWriteService.UpdateCategory(category);
            return Ok(res);
        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            try
            {
                Category res = await _taskWriteService.AddCategory(category);
                return Ok(res);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        // DELETE: api/Category/5
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            await _taskWriteService.DeleteCategory(categoryId);

            return NoContent();
        }

        //[HttpPut("changeOrder")]
        //public async Task<IActionResult> PostCategory(IEnumerable<Category> categories)
        //{
        //    var unit = await _taskPageHelper.GetUnit();
        //    var ctgIds = categories.Select(c => c.Id).ToList();
        //    var visibilityArray = categories.Select(c => c.IsVisible).ToList();

        //    foreach (var c in unit.Categories)
        //    {
        //        int index = ctgIds.IndexOf(c.Id);
        //        c.Position = index;
        //        c.IsVisible = visibilityArray[index];
        //        c.ColorHex = categories.ElementAt(index).ColorHex;
        //    }

        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}

        //[HttpPut("clearArchive")]
        //public async Task<IActionResult> ClearArchive()
        //{
        //    var unit = await _taskPageHelper.GetUnit();

        //    _context.Entry(unit).State = EntityState.Modified;

        //    unit.TaskArchive = new List<ArchivedTask>();
        //    unit.Statistic.PerDayStatistic = new List<Models.DonePerDay>();

        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
