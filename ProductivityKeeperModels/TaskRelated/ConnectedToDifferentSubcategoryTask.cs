using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels.TaskRelated
{
    public class ConnectedToDifferentSubcategoryTask
    {
        public int Id { get; set; }
        public int SubcategoryId1 { get; set; }
        public int SubcategoryId2 { get; set; }
    }
}
