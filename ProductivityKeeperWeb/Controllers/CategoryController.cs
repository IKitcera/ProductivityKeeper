using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ITaskPageHelper _taskPageHelper;   
        public CategoryController(ApplicationContext context, ITaskPageHelper _helper)
        {
            _context = context;
            _taskPageHelper = _helper;  
        }


        [HttpGet]
        public async Task<ActionResult<Unit>> GetCategories()
        {
            var unit = await _taskPageHelper.GetUnit();
            if (unit == null)
                return Unauthorized();

            return unit;
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int categoryId)
        {
            var unit = await _taskPageHelper.GetUnit();
            if (unit == null)
                return Unauthorized();

            var category = unit.Categories.FirstOrDefault(cat => cat.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int categoryId, Category category)
        {
            if (categoryId != category.Id)
            {
                return BadRequest();
            }
            
            var unit = await _taskPageHelper.GetUnit();
           
            if (unit == null)
                return Unauthorized();

            _context.Entry(unit).State = EntityState.Modified;

            try
            {
               
                var ctg = unit.Categories.FirstOrDefault(c => c.Id == categoryId);
                ctg = category;


                var u = await _context.Units.FindAsync(unit.Id);
                u = unit;
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(categoryId))
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

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category Category)
        {
            var unit = await _taskPageHelper.GetUnit();
            if (unit == null)
                return Unauthorized();

            Category = _taskPageHelper.FillCategory(Category);
            if (Category == null)
                return BadRequest();

            unit.Categories.Add(Category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(Category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCategory", new { id = Category.Id }, Category);
        }

        // DELETE: api/Category/5
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var unit = await _taskPageHelper.GetUnit();
            if (unit == null)
                return Unauthorized();

            var Category = unit.Categories.FirstOrDefault(cat => cat.Id == categoryId);

            if (Category == null)
            {
                return NotFound();
            }

            unit.Categories.Remove(Category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int categoryId)
        {
            Unit unit = null;
            System.Threading.Tasks.Task.Run(async () => unit = await _taskPageHelper.GetUnit());

            if (unit != null)
                return unit.Categories.Any(cat => cat.Id == categoryId);

            return false;
        }
    }
}
