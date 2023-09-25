using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IStatistics _analytic;
        public AnalyticsController(IStatistics analytic)
        {
            _analytic = analytic;
        }

        [HttpGet]
        public async Task<ActionResult<UserStatistic>> GetStatistic()
        {
            return await _analytic.GetStatistic();
        }
    }
}
