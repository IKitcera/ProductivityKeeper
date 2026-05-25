using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductivityKeeperWeb.Domain.DTO;
using ProductivityKeeperWeb.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DiaryController : ControllerBase
    {
        private readonly DiaryService _service;

        public DiaryController(DiaryService service)
        {
            _service = service;
        }

        // GET: api/Diary
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaryItemDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // GET: api/Diary/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryItemDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        // POST: api/Diary
        [HttpPost]
        public async Task<ActionResult<DiaryItemDto>> Create(DiaryItemDto dto)
        {
            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Diary/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiaryItemDto dto)
        {
            if (id != dto.Id)
                return BadRequest();
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        // DELETE: api/Diary/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
