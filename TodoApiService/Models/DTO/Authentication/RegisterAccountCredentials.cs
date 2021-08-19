using System.ComponentModel.DataAnnotations;

namespace TodoApiService.Models.DTO.Authentication
{
    public class RegisterAccountCredentials
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}