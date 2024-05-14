using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.DTO;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Utils;
using ProductivityKeeperWeb.Hubs;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserTasksProgressPredictionEngine;
using UserTasksProgressPredictionEngine.Models;

namespace ProductivityKeeperWeb.Services
{
    public class StatisticsService : IStatistics
    {
        private readonly ITasksReadService _taskReadService;
        private readonly ITasksWriteService _taskWriteService;
        private readonly IHubContext<ChartHub> _chartHubContext;
        private readonly string _connectionString;

        public StatisticsService(
            ITasksReadService taskReadService,
            ITasksWriteService taskWriteService,
            IHubContext<ChartHub> chartHubContext,
            IConfiguration configuration)
        {
            _taskReadService = taskReadService;
            _taskWriteService = taskWriteService;
            _chartHubContext = chartHubContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<UserStatistic> GetStatistic()
        {
            var statistic = await _taskReadService.GetStatistic();
            return statistic ?? await CountStatistic();
        }

        public async Task<ForecastedStatisticResult> GetStatisticWithPrediction()
        {
            var stat = await _taskReadService.GetStatistic();

            if (stat.PerDayStatistic.Count <= 10)
            {
                throw new ArgumentException($"At least 11 days of statistic are required to predict future values. Now {stat.PerDayStatistic.Count} days are counted");
            }

            var pe = new StatisticPredictionEngine();
            var predictionRes = pe.Predict(_connectionString, stat.Id, 10);
            return predictionRes;
        }

        public async Task<UserStatistic> CountStatistic(int? unitId = null)
        {
            var unit = await _taskReadService.GetUnit(unitId);

            var statistic = unit.Statistic;

            statistic ??= await _taskWriteService.FillNewStatistic(unit);

            StatisticsUtil.CountBaseStatistic(unit, statistic);

            statistic = await _taskWriteService.UpdateUserStatistic(statistic);

            // For showing today value
            if (!statistic.PerDayStatistic.Any() ||
                statistic.PerDayStatistic.Last().Date.Date != DateTime.Now.Date)
            {
                statistic.PerDayStatistic.Add(new DonePerDay { Date = DateTime.Now.Date, CountOfDone = 0 });
            }

            statistic.PerDayStatistic.ForEach(pds => pds.Statistic = null);

            _chartHubContext.Clients.Client(ChartHub.UnitConnectionsToUnits[unit.UserId])
                .SendAsync("StatisticUpdated", statistic);

            return statistic;
        }

        public async Task<AverageStatisticDTO> GetAverageStatistic()
        {
            var statId = await _taskReadService.GetStatisticID();
            var donePerDaysData = await _taskReadService.GetPerDayStatistic();

            return StatisticsUtil.CalculateAverageStatistic(donePerDaysData, statId);
        }
    }
}
