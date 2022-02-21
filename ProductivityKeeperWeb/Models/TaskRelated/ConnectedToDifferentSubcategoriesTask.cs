using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    public class ConnectedToDifferentSubcategoriesTask
    {
        public int Id { get; set; }
        public int SubcategoryId1 { get; set; }
        public int SubcategoryId2 { get; set; }
    }
}
