using System.ComponentModel.DataAnnotations;

namespace TodoApiService.Models.DTO
{
    public class EditTodoItem
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public bool IsImportant { get; set; }

    }
}