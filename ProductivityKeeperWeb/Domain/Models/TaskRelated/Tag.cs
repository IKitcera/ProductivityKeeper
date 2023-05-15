namespace ProductivityKeeperWeb.Domain.Models.TaskRelated
{
    public class Tag
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public int TaskId { get; set; }
        public string Text { get; set; }
        public string ColorHex { get; set; }

        public static Tag GetTag(int taskId, Subcategory s) =>
            new Tag
            {
                TaskId = taskId,
                SubcategoryId = s.Id,
                CategoryId = s.CategoryId,
                Text = s.Name,
                ColorHex = s.Category?.ColorHex
            };
    }
}
