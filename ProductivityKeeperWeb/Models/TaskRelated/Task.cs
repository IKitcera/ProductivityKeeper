using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    [Owned]
    public class Task
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }
        public DateTime ? Deadline { get; set; }
        public DateTime ? DoneDate { get; set; }
        public Task()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
