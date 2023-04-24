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
        public string ColorHex { get; set; }

        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();


        //todo: remove
        [JsonIgnore]
        public int Position { get; set; }
        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }
      

        public Subcategory()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
