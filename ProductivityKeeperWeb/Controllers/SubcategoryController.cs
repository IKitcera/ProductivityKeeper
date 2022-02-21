using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models.TaskRelated;
using ProductivityKeeperWeb.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SubcategoryController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ITaskPageHelper _taskPageHelper; 
        public SubcategoryController(ApplicationContext context, ITaskPageHelper _helper)
        {
            _context = context;
            _taskPageHelper = _helper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subcategory>>> GetSubcategories(int categoryId)
        {
            var ctg = await _taskPageHelper.GetCategory(categoryId);
            return ctg.Subcategories;
        }

        // GET: api/Categorys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategory>> GetSubategory(int categoryId, int subcategoryId)
        {
            var ctg = await _taskPageHelper.GetCategory(categoryId);
            var sub = ctg.Subcategories.FirstOrDefault(scat => scat.Id == subcategoryId);

            if (sub == null)
            {
                return NotFound();
            }

            return sub;
        }

        // PUT: api/Categorys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubcategory(int categoryId, int subcategoryId, Subcategory subcategory)
        {
            if (subcategoryId != subcategory.Id)
            {
                return BadRequest();
            }

            var unit = _taskPageHelper.GetUnit();

            _context.Entry(unit).State = EntityState.Modified;

            try
            {
                var ctg = await _taskPageHelper.GetCategory(categoryId);
                var sub = ctg.Subcategories.FirstOrDefault(scat => scat.Id == subcategoryId);
                sub = subcategory;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubcategoryExists(categoryId, subcategoryId))
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
        public async Task<ActionResult<Subcategory>> PostSubcategory(int categoryId, Subcategory Subcategory)
        {
            var ctg = await _taskPageHelper.GetCategory(categoryId);
            Subcategory = _taskPageHelper.FillSubcategory(categoryId, Subcategory);

            if (Subcategory == null)
                return BadRequest();

            ctg.Subcategories.Add(Subcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubcategories", new { id = Subcategory.Id }, Subcategory);
        }

        // DELETE: api/Categorys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategory(int categoryId, int subcategoryId)
        {
            var ctg = await _taskPageHelper.GetCategory(categoryId);
            var sub = ctg.Subcategories.FirstOrDefault(scat => scat.Id == subcategoryId);


            if (sub == null)
            {
                return NotFound();
            }

            ctg.Subcategories.Remove(sub);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubcategoryExists(int categoryId, int subcategoryId)
        {
            Category ctg = null;
            System.Threading.Tasks.Task.Run(async () => ctg = await _taskPageHelper.GetCategory(categoryId));

            if (ctg != null)
                return ctg.Subcategories.Any(cat => cat.Id == categoryId);

            return false;
        }

       
    }
}
