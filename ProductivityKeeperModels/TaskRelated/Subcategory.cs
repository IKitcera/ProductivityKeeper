using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels.TaskRelated
{
    public class Subcategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        public DateTime DateOfCreation { get; private set; }
        public Subcategory()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
