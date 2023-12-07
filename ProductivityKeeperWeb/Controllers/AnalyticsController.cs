using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductivityKeeperWeb.Domain.DTO;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;
using UserTasksProgressPredictionEngine.Models;

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

        [HttpGet("statistic-and-predictions")]
        public async Task<ActionResult<ForecastedStatisticResult>> GetStatisticWithPrediction()
        {
            return await _analytic.GetStatisticWithPrediction();
        }

        [HttpGet("average-users-statistic")]
        public async Task<ActionResult<AverageStatisticDTO>> GetAverageStatistic()
        {
            return await _analytic.GetAverageStatistic();
        }
    }
}
