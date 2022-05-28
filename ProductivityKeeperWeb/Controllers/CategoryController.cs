using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetJustCategories()
        {
            var unit = await _taskPageHelper.GetUnit();
            if (unit == null)
                return Unauthorized();

            return unit.Categories;
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
               
                var ctg = unit.Categories.Where(c => c.Id == categoryId).First();
                ctg.Name = category.Name;
                ctg.Color = category.Color;
                ctg.Subcategories = category.Subcategories;
                ctg.Subcategories.ForEach(sub =>
                {
                    var inputMatch = category.Subcategories.FirstOrDefault(s => s.Id == sub.Id);
                    if (inputMatch != null)
                    {
                        sub.Position = category.Subcategories.IndexOf(inputMatch);
                    }
                });

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
            unit.Categories.ForEach(c =>
            {
                c.Position = unit.Categories.IndexOf(c);
            });

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

        [HttpPut("changeOrder")]
        public async Task<IActionResult> PostCategory(IEnumerable<Category> categories)
        {
            var unit = await _taskPageHelper.GetUnit();
            var ctgIds = categories.Select(c => c.Id).ToList();
            var visibilityArray = categories.Select(c => c.IsVisible).ToList();

            foreach (var c in unit.Categories)
            {
                int index = ctgIds.IndexOf(c.Id);
                c.Position = index;
                c.IsVisible = visibilityArray[index];
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

            // DELETE: api/Category/5
            [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var unit = await _taskPageHelper.GetUnit();
            if (unit == null)
                return Unauthorized();

            var category = unit.Categories.FirstOrDefault(cat => cat.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            if(category.Subcategories.Any())
            {
                foreach(var sub in category.Subcategories)
                {
                    if (sub.Tasks.Any())
                    {
                        foreach (var task in sub.Tasks)
                            await _taskPageHelper.DeleteRelatedTasks(categoryId, sub.Id, task.Id);
                    }
                }
            }

            unit.Categories.Remove(category);
            unit.Categories.ForEach(c =>
            {
                c.Position = unit.Categories.IndexOf(c);
            });
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
