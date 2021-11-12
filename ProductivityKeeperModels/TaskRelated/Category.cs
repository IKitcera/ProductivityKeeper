using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels.TaskRelated
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        public DateTime DateOfCreation { get; private  set; }

        public Category()
        {
            DateOfCreation = DateTime.Now;
        }
    }
}
