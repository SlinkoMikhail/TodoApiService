using TodoApiService.Models.DTO.Authentication;

namespace TodoApiService.Models
{
    public interface IAccountManager
    {
        bool RegisterAccount(RegisterAccountCredentials registerCredentials);
        TokenResult LoginAccount(LoginAccountCredentials loginCredentials);
    }
}