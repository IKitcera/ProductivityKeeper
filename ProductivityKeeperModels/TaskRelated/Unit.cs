using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels.TaskRelated
{
    public class Unit
    {
        public int Id { get; set; }
        public List<Category> Categories = new List<Category>();
    }
}
