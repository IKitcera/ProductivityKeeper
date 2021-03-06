using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Services;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TimerController : ControllerBase
    {
        private readonly ApplicationContext context;
        private readonly ITaskPageHelper helper;
        public TimerController(ApplicationContext context, ITaskPageHelper helper)
        {
            this.context = context;
            this.helper = helper;
        }
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult<Timer>> GetTimer()
        {
            var unit = await helper.GetUnit();
            return unit.Timer;
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> SetTimer(Timer timer)
        {
            var unit = await helper.GetUnit();

            context.Entry(unit).State = EntityState.Modified;
            unit.Timer = timer;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("/update-ticked")]
        public async System.Threading.Tasks.Task<IActionResult> SetTimer(long tickedSeconds)
        {
            var unit = await helper.GetUnit();
            context.Entry(unit).State = EntityState.Modified;
            unit.Timer.Ticked = tickedSeconds;
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("update-format")]
        public async System.Threading.Tasks.Task<IActionResult> UpdateFormat(int newFormat)
        {
            var unit = await helper.GetUnit();
            context.Entry(unit).State = EntityState.Modified;
            unit.Timer.Format = newFormat;
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
