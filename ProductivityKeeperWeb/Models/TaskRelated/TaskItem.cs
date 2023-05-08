using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProductivityKeeperWeb.Models.TaskRelated
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
        //-----Connected tasks
        public int? RelationId { get; set; }

        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();


        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }
        public TaskItem()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
