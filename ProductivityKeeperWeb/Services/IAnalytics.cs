using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Services
{
    public interface IAnalytics
    {
        Task<UserStatistic> GetStatistic(int unitId);
        Task<UserStatistic> CountStatistic(int unitId);
    }
}
