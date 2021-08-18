namespace TodoApiService.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public bool IsImportant { get; set; }
        public System.Guid AccountId { get; set; }
    }
}