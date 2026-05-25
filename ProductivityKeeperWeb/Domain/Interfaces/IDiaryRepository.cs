using System.Collections.Generic;
using System.Threading.Tasks;
using ProductivityKeeperWeb.Domain.Models;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface IDiaryRepository
    {
        Task<IEnumerable<DiaryItem>> GetAllAsync(int unitId);
        Task<DiaryItem> GetByIdAsync(int id, int unitId);
        Task<DiaryItem> AddAsync(DiaryItem item);
        Task UpdateAsync(DiaryItem item);
        Task DeleteAsync(int id, int unitId);
    }
}
