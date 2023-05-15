namespace ProductivityKeeperWeb.Domain.Models.TaskRelated
{
    public class SubcategoryTask
    {
        public int SubcategoryId { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
        public Subcategory Subcategory { get; set; }
    }
}