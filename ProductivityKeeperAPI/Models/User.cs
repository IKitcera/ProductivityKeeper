
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductivityKeeperWeb.Models
{
    public class User: AbstractUser
    {
        public Roles Role { get; set; }
        public int UnitId { get; set; }
        public UserSettings UserSettings { get; set; } = new UserSettings();
        public DateTime RegistrationDate { get; set; }
        public enum Roles
        {
            Admin,
            UserPro,
            User
        }
    }

}
