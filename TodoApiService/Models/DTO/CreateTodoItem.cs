using System.ComponentModel.DataAnnotations;

namespace TodoApiService.Models.DTO
{
    public class CreateTodoItem
    {
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public bool IsImportant { get; set; }
        [Required]
        [MinLength(4)]
        public string Title { get; set; }
    }
}