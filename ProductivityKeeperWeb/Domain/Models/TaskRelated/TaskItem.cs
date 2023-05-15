using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProductivityKeeperWeb.Domain.Models.TaskRelated
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }

        public DateTime? Deadline { get; set; }
        public DateTime? DoneDate { get; set; }

        //-----Habbits
        public bool IsRepeatable { get; set; } = false;
        public int? TimesToRepeat { get; set; }
        public int? GoalRepeatCount { get; set; }

        public double? HabbitIntervalInHours { get; set; }
        
        public List<Subcategory> Subcategories { get; set; } = new ();
        public ICollection<Tag> Tags { get => Subcategories
                .Select(s => Tag.GetTag(Id, s)).ToList(); } 

        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }
        public TaskItem()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
