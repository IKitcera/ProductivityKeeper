using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Services
{
    public class TimerService : ITimerService
    {
        private readonly ApplicationContext _context;
        private readonly IAuthService _authService;
        public TimerService(
            ApplicationContext contxt,
            IAuthService authService)
        {
            _context = contxt;
            _authService = authService;
        }

        public async Task<Timer> GetTimer()
        {
            var res = await _context.Timers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UnitId == _authService.GetUnitId());

            //TODO: remove later
            if (res == null)
            {
                res = new Timer { UnitId = _authService.GetUnitId() };

                _context.Entry(res).State = EntityState.Added;

                await _context.SaveChangesAsync();
            }

            return res;
        }


        public async Task<Timer> SetTimer(Timer timer)
        {
            var item = await _context.Timers.FindAsync(timer.Id);
            
            item.UnitId = timer.UnitId > 0 ? timer.UnitId : _authService.GetUnitId();
            item.Ticked = timer.Ticked;
            item.Label = timer.Label;
            item.Format = timer.Format;
            item.Goal = timer.Goal;

            _context.Entry(item).State = EntityState.Modified;
           
            await _context.SaveChangesAsync();
            return await GetTimer();
        }


        public async Task UpdateTicked(long seconds)
        {
            var item = await _context.Timers
                .FirstOrDefaultAsync(t => t.UnitId == _authService.GetUnitId());

            item.Ticked = seconds;

            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateFormat(int newFormat)
        {
            var item = await _context.Timers
                .FirstOrDefaultAsync(t => t.UnitId == _authService.GetUnitId());

            item.Format = newFormat;

            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
    }
}
