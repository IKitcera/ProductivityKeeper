using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface IAnalytics
    {
        Task<UserStatistic> GetStatistic();
        Task<UserStatistic> CountStatistic(Unit unit = null);
    }
}
