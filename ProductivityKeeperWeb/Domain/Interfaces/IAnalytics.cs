using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface IAnalytics
    {
        Task<UserStatistic> GetStatistic(int unitId);
        Task<UserStatistic> CountStatistic(int unitId);
    }
}
