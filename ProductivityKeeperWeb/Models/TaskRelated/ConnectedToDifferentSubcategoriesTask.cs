using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    [Owned]
    public class ConnectedToDifferentSubcategoriesTask : Task
    {
        public List<int> CategoriesId { get; set; } = new List<int>();
        public List<int> SubcategoriesId { get; set; } = new List<int>();
    }
}
