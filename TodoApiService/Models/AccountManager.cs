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
            Claim[] claims = new[]
            {
                new Claim(ClaimNames.UniqueId, account.Id.ToString())
            };
            var signingCredentials = new SigningCredentials(_jwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtAuthOptions.Issuer,
                claims: claims,
                audience: _jwtAuthOptions.Audience,
                expires: DateTime.UtcNow.AddSeconds(_jwtAuthOptions.TokenLifeTime),
                signingCredentials: signingCredentials
            );
            tokenResult.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
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
            }
            await _appDbContext.SaveChangesAsync();
            tokenResult.RefreshToken = refreshToken.Token;
            return tokenResult;
        }
        public async Task<Account> GetAccountByIdAsync(Guid id) => await _appDbContext.Accounts.FindAsync(id);
        public async Task<TokenResult> LoginAccount(LoginAccountCredentials loginCredentials)
        {
            Account user = _appDbContext.Accounts.FirstOrDefault(a => 
                (loginCredentials.EmailOrPhone.Trim().ToLowerInvariant() == a.Email 
                || loginCredentials.EmailOrPhone.Trim().ToLowerInvariant() == a.Phone));
            if(user == null || !BCrypt.Net.BCrypt.Verify(loginCredentials.Password, user.HashPassword))
                throw new SecurityTokenException("Incorrect password or this email is not registered.");
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
                .FirstOrDefault(a => a.Email == registerCredentials.Email.Trim().ToLowerInvariant() 
                || a.Phone == registerCredentials.Phone.Trim().ToLowerInvariant());
            if(user != null) throw new SecurityTokenException("This email or phone number is already in use.");
            user = new Account
            {
                Email = registerCredentials.Email.Trim().ToLowerInvariant(),
                Phone = registerCredentials.Phone.Trim().ToLowerInvariant(),
                HashPassword = BCrypt.Net.BCrypt.HashPassword(registerCredentials.Password),
                Role = Roles.User
            };
            await _appDbContext.Accounts.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public Account GetAccountById(Guid id) => _appDbContext.Accounts.Find(id);
    }
}