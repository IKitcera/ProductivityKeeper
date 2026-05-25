using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Interfaces;
using System.Linq;

namespace ProductivityKeeperWeb.Services.Repositories
{
    public class DiaryRepository : IDiaryRepository
    {
        private readonly ApplicationContext _context;
        public DiaryRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiaryItem>> GetAllAsync(int unitId)
        {
            return await _context.DiaryItems.AsNoTracking().Where(d => d.UnitId == unitId).ToListAsync();
        }

        public async Task<DiaryItem> GetByIdAsync(int id, int unitId)
        {
            return await _context.DiaryItems.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id && d.UnitId == unitId);
        }

        public async Task<DiaryItem> AddAsync(DiaryItem item)
        {
            _context.DiaryItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(DiaryItem item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int unitId)
        {
            var item = await _context.DiaryItems.FirstOrDefaultAsync(d => d.Id == id && d.UnitId == unitId);
            if (item != null)
            {
                _context.DiaryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
