using Microsoft.EntityFrameworkCore;
using System;

namespace ProductivityKeeperWeb.Domain.Models
{
    [Owned]
    public class DonePerDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CountOfDone { get; set; }
    }
}
