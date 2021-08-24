using TodoApiService.Models.DTO.Authentication;

namespace TodoApiService.Models
{
    public interface IAccountManager
    {
        bool RegisterAccount(RegisterAccountCredentials registerCredentials);
        Account LoginAccount(LoginAccountCredentials loginCredentials);
        TokenResult GenerateJWTTokens(Account account);
    }
}