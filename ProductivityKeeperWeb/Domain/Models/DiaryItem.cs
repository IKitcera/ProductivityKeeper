using System;

namespace ProductivityKeeperWeb.Domain.Models
{
    public class DiaryItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UnitId { get; set; }
    }
}
