using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;

namespace ProductivityKeeperWeb.Services
{
    public interface IAnalytics
    {
        System.Threading.Tasks.Task<UserStatistic> GetStatistic();
    }
}
