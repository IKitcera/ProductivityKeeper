using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    [Owned]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        public bool IsVisible { get; set; } 
        [JsonIgnore]
        public int Position { get; set; }
        [JsonIgnore]
        public DateTime DateOfCreation { get; set; }

    }
}
