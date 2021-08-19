namespace TodoApiService.Models.DTO.Authentication
{
    public class TokenResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}