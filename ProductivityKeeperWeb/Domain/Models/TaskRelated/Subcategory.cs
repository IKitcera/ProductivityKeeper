using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductivityKeeperWeb.Domain.Models.TaskRelated
{
    public class Subcategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();


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

