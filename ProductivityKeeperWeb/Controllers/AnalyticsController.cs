using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductivityKeeperWeb.Models;
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
        private readonly ITaskPageHelper _helper;
        public AnalyticsController(IAnalytics _analytic, ITaskPageHelper _helper)
        {
            this._helper = _helper;
            this._analytic = _analytic;
        }

        [HttpGet]
        public async Task<ActionResult<UserStatistic>> GetStatistic()
        {
            var unit = await _helper.GetUnit();
            var statistic = _analytic.GetStatistic(unit);
            return statistic;
        }
    }
}
