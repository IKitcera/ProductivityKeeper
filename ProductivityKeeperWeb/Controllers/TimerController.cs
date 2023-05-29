using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TimerController : ControllerBase
    {
        private readonly ITimerService _timerService;

        public TimerController(ITimerService timerService)
        {
            _timerService = timerService;
        }

        [HttpGet]
        public async Task<ActionResult<Timer>> GetTimer()
        {
           return await _timerService.GetTimer();
        }

        [HttpPost]
        public async Task<Timer> SetTimer(Timer timer)
        {
            return await _timerService.SetTimer(timer);
        }

        [HttpPost("update-ticked")]
        public async Task<IActionResult> SetTimer(long tickedSeconds)
        {
            await _timerService.UpdateTicked(tickedSeconds);
            return Ok();
        }

        [HttpPut("update-format")]
        public async Task<IActionResult> UpdateFormat(int newFormat)
        {
            await _timerService.UpdateFormat(newFormat);
            return Ok();
        }
    }
}
