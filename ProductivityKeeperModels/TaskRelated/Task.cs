using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels.TaskRelated
{
    public class Task
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
        public DateTime DateOfCreation { get; private set; }
        public DateTime Deadline { get; set; }
        public DateTime DoneDate { get; set; }
        public Task()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
