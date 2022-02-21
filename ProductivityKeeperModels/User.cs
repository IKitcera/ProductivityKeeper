using ProductivityKeeperModels.TaskRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperModels
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HashPassword { get; set; }
        public int UserSettingsId { get; set; }
        public int UnitId { get; set; }
        public UserSettings UserSettings { get; set; } = new UserSettings();
        
        public DateTime RegistrationDate { get; private set; }
    }
}
