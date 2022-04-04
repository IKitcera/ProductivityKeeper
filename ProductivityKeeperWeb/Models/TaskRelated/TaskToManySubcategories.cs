using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    [Owned]
    public class TaskToManySubcategories
    {
        public int Id { get; set; }
        public List<TaskSubcategory> TaskSubcategories { get; set; } = new List<TaskSubcategory>();
    }

    [Owned]

    public class TaskSubcategory
    {
        public int TaskId { get; set; }
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
    }
}
