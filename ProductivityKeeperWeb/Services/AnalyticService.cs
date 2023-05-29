using Microsoft.EntityFrameworkCore;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using ProductivityKeeperWeb.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Services
{
    public class AnalyticService : IAnalytics
    {
        private readonly ITasksReadService _taskReadService;
        private readonly ITasksWriteService _taskWriteService;
        private readonly ApplicationContext _app; //TODO:REMOVE after

        public AnalyticService(
            ITasksReadService taskReadService,
            ITasksWriteService taskWriteService,
            ApplicationContext app)
        {
            _taskReadService = taskReadService;
            _taskWriteService = taskWriteService;
            _app = app;
        }

        public async Task<UserStatistic> GetStatistic()
        {
            var statistic = await _taskReadService.GetStatistic();
            return statistic ?? await CountStatistic();
        }

        public async Task<UserStatistic> CountStatistic(Unit unit = null)
        {
            unit ??= await _taskReadService.GetUnit();

            var statistic = await _taskReadService.GetStatistic();

            //TODO: Remove

            if (statistic == null)
            {
                using var transaction = await _app.Database.BeginTransactionAsync();

                try
                {
                    statistic = new UserStatistic { UnitId = unit.Id };
                var res = await _app.Statistics.AddAsync(statistic);

                await _app.SaveChangesAsync();

                unit.StatisticId = statistic.Id;

                _app.Units.Entry(unit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _app.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // Rollback the transaction in case of any exception
                    await transaction.RollbackAsync();
                    throw; // Rethrow the exception to be handled at a higher level
                }
            }

            AnalysisUtil.CountStatistic(unit, statistic);

            statistic = await _taskWriteService.UpdateUserStatistic(statistic);

            // For showing today value
            if (!statistic.PerDayStatistic.Any() ||
                statistic.PerDayStatistic.Last().Date.Date != DateTime.Now.Date)
            {
                statistic.PerDayStatistic.Add(new DonePerDay { Date = DateTime.Now.Date, CountOfDone = 0 });
            }
            return statistic;
        }
    }
}
