using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    public class Unit
    {
        public int Id { get; set; }
        [JsonIgnore]
        public string UserId { get; init; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public Timer Timer { get; set; } = new Timer();
    }
}
