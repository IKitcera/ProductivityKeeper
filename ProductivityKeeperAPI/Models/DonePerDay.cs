using Microsoft.EntityFrameworkCore;
using System;

namespace ProductivityKeeperWeb.Models
{
  [Owned]
    public class DonePerDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CountOfDone { get; set; }
    }
}
