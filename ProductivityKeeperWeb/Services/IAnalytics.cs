using ProductivityKeeperWeb.Models;
using ProductivityKeeperWeb.Models.TaskRelated;

namespace ProductivityKeeperWeb.Services
{
    public interface IAnalytics
    {
        UserStatistic GetStatistic(Unit usersUnit);
    }
}
