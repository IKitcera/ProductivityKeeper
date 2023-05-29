using ProductivityKeeperWeb.Domain.Models;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface ITimerService
    {
        Task<Timer> GetTimer();
        Task<Timer> SetTimer(Timer timer);
        Task UpdateTicked(long seconds);
        Task UpdateFormat(int newFormat);
    }
}
