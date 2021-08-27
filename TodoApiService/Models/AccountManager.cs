using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApiService.Models.DTO.Authentication;
using TodoApiService.Models.Options;

namespace TodoApiService.Models
{
    public class AccountManager : IAccountManager
    {
        private readonly JWTAuthOptions _jwtAuthOptions;
        private readonly TodoApiApplicationContext _appDbContext;
        public AccountManager(IOptions<JWTAuthOptions> jwtAuthOptions, TodoApiApplicationContext appDbContext)
        {
            _jwtAuthOptions = jwtAuthOptions.Value;
            _appDbContext = appDbContext;
        }
        private async Task<TokenResult> GenerateJWTTokens(Account account)
        {
            //access_token
            TokenResult tokenResult = new TokenResult();
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtAuthOptions.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtAuthOptions.Audience)
                }), 
                Expires = DateTime.UtcNow.AddSeconds(_jwtAuthOptions.TokenLifeTime),
                SigningCredentials = new SigningCredentials(_jwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            };
            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            tokenResult.AccessToken = jwtTokenHandler.WriteToken(token);
            //refresh_token 
            RefreshToken refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                IsRevoked = false,
                IsUsed = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Account = account,
                AccountId = account.Id
            };
            RefreshToken refreshTokenStored = _appDbContext.RefreshTokens.FirstOrDefault(t => t.AccountId == account.Id);
            if(refreshTokenStored == null)
            {
                await _appDbContext.RefreshTokens.AddAsync(refreshToken);
            }
            else
            {
                refreshTokenStored.AddedDate = refreshToken.AddedDate;
                refreshTokenStored.ExpiryDate = refreshToken.ExpiryDate;
                refreshTokenStored.IsRevoked = refreshToken.IsRevoked;
                refreshTokenStored.IsUsed = refreshToken.IsUsed;
                refreshTokenStored.Token = refreshToken.Token;
                _appDbContext.RefreshTokens.Update(refreshTokenStored);
            }
            await _appDbContext.SaveChangesAsync();
            tokenResult.RefreshToken = refreshToken.Token;
            return tokenResult;
        }
        public async Task<Account> GetAccountByIdAsync(Guid id) => await _appDbContext.Accounts.FindAsync(id);
        public async Task<TokenResult> LoginAccount(LoginAccountCredentials loginCredentials)
        {
            Account user = _appDbContext.Accounts.FirstOrDefault(a => 
                (loginCredentials.EmailOrPhone == a.Email || loginCredentials.EmailOrPhone == a.Phone) 
                && a.HashPassword == loginCredentials.Password);
            if(user == null) return null;
            return await GenerateJWTTokens(user);
        }
        public async Task<TokenResult> RefreshJWTTokens(string refreshToken)
        {
            RefreshToken refresh = _appDbContext.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if(refresh != null && refresh.ExpiryDate > DateTime.UtcNow && refresh.IsRevoked == false && refresh.IsUsed == false)
            {
                refresh.IsUsed = true;
                Account user = await _appDbContext.Accounts.FindAsync(refresh.AccountId);
                TokenResult tokenResult = await this.GenerateJWTTokens(user);
                if(tokenResult != null)
                    return tokenResult;
            }
            return null;
        }

        public async Task<bool> RegisterAccount(RegisterAccountCredentials registerCredentials)
        {
            Account user = _appDbContext.Accounts
                .FirstOrDefault(a => a.Email == registerCredentials.Email || a.Phone == registerCredentials.Phone);
            if(user != null) return false;
            user = new Account
            {
                Email = registerCredentials.Email,
                Phone = registerCredentials.Phone,
                HashPassword = registerCredentials.Password,
                Role = Roles.User
            };
            await _appDbContext.Accounts.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public Account GetAccountById(Guid id) => _appDbContext.Accounts.Find(id);
    }
}