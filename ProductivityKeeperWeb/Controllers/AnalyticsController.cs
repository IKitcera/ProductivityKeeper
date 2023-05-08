using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Repositories.Interfaces;
using ProductivityKeeperWeb.Services;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalytics _analytic;
        public AnalyticsController(IAnalytics analytic)
        {
            _analytic = analytic;
        }

        [HttpGet]
        public async Task<ActionResult<UserStatistic>> GetStatistic(int unitId)
        {
            return await _analytic.GetStatistic(unitId);
        }
    }
}
