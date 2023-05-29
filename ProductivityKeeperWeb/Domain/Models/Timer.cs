using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductivityKeeperWeb.Domain.Models.TaskRelated;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductivityKeeperWeb.Domain.Models
{
    [Owned]
    public class Timer
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public long Ticked { get; set; }
        public long Goal { get; set; }
        public int Format { get; set; }

        public int UnitId { get; set; }
        [JsonIgnore]
        public Unit Unit { get; set; }
    }
}
