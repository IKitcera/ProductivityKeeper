using System.ComponentModel.DataAnnotations;

namespace ProductivityKeeperWeb.Domain.Models
{
    public class AbstractUser
    {
        [Key]
        [EmailAddress]
        public string Email { get; set; }
        public string HashPassword { get; set; }
        //  public int UnitId { get; set; }
    }
}
