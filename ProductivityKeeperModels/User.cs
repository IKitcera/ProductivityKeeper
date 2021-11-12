using ProductivityKeeperModels.TaskRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels
{
    public class User
    {
        public string Name { get; set; }
        public string HashPassword { get; set; }
        public UserSettings UserSettings { get; set; } = new UserSettings();
        public List<Category> Categories { get; set; } = new List<Category>();
        public DateTime RegistrationDate { get; private set; }
    }
}
