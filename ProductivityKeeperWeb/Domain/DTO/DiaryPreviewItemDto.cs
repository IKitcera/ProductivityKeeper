using System;

namespace ProductivityKeeperWeb.Domain.DTO
{
    public class DiaryPreviewItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? ContentPreview { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
