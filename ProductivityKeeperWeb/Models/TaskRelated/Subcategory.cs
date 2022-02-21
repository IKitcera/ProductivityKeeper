using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    [Owned]
    public class Subcategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }
        public Subcategory()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
