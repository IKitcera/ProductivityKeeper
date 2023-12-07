using ProductivityKeeperWeb.Domain.DTO;
using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;
using UserTasksProgressPredictionEngine.Models;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface IStatistics
    {
        Task<UserStatistic> GetStatistic();
        Task<ForecastedStatisticResult> GetStatisticWithPrediction();
        Task<AverageStatisticDTO> GetAverageStatistic();
        Task<UserStatistic> CountStatistic(int? unitId = null);
    }
}
