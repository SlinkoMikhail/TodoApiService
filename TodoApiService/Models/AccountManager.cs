using TodoApiService.Models.DTO.Authentication;

namespace TodoApiService.Models
{
    public class AccountManager : IAccountManager
    {
        public TokenResult GenerateJWTTokens(Account account)
        {
            throw new System.NotImplementedException();
        }

        public TokenResult LoginAccount(LoginAccountCredentials loginCredentials)
        {
            throw new System.NotImplementedException();
        }

        public bool RegisterAccount(RegisterAccountCredentials registerCredentials)
        {
            throw new System.NotImplementedException();
        }
    }
}