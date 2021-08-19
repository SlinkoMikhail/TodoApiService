using System.ComponentModel.DataAnnotations;

namespace TodoApiService.Models.DTO.Authentication
{
    public class AccountCredentials
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}