using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProductivityKeeperWeb.Domain.Models.TaskRelated
{
    public class Unit
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string UserId { get; init; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public Timer Timer { get; set; } = new Timer();

        [ForeignKey(nameof(StatisticId))]
        public int StatisticId { get; set; }
        //  public UserStatistic Statistic { get; set; }
        public List<ArchivedTask> TaskArchive { get; set; } = new List<ArchivedTask>();
    }
}
