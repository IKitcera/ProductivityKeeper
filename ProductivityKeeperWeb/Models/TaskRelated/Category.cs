using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; }
        public bool IsVisible { get; set; }
        public int UnitId { get; set; }

        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();

        //todo: remove
        [JsonIgnore]
        public int Position { get; set; }
        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }


    }
}
