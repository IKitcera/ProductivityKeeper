using System.ComponentModel.DataAnnotations;

namespace ProductivityKeeperWeb.Models
{
    public class AbstractUser
    {
        [Key]
        public string Email { get; set; }
        public string HashPassword { get; set; }
      //  public int UnitId { get; set; }
    }
}
