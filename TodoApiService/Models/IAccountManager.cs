using System;
using System.Threading.Tasks;
using TodoApiService.Models.DTO.Authentication;

namespace TodoApiService.Models
{
    public interface IAccountManager
    {
        Task<bool> RegisterAccount(RegisterAccountCredentials registerCredentials);
        Task<TokenResult> LoginAccount(LoginAccountCredentials loginCredentials);
        Task<TokenResult> RefreshJWTTokens(string refreshToken);
        Task<Account> GetAccountByIdAsync(Guid id);
        Account GetAccountById(Guid id);
    }
}