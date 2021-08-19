using System.ComponentModel.DataAnnotations;

namespace TodoApiService.Models.DTO.Authentication
{
    public class LoginAccountCredentials
    {
        [Required]
        public string EmailOrPhone { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}