using System;

namespace ProductivityKeeperWeb.Domain.DTO
{
    public class DiaryItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
