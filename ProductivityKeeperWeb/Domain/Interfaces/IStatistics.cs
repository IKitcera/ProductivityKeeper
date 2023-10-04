﻿using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb.Domain.Interfaces
{
    public interface IStatistics
    {
        Task<UserStatistic> GetStatistic();
        Task<UserStatistic> CountStatistic(int? unitId = null);
    }
}