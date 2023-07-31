using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;

namespace ProductivityKeeperWeb.Domain.Models
{
    public class DonePerDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CountOfDone { get; set; }
        public int StatisticId { get; set; }
        [JsonIgnore]
        public UserStatistic Statistic { get; set; }
    }
}
