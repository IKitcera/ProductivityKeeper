using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Services
{
    public interface IAnalytics
    {
        Task<UserStatistic> GetStatistic(int unitId);
        Task<UserStatistic> CountStatistic(int unitId);
    }
}
