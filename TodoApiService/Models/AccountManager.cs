using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        public TokenResult GenerateJWTTokens(Account account)
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
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtAuthOptions.Audience),
                    new Claim(JwtRegisteredClaimNames.Email, account.Email?.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
            tokenResult.RefreshToken = refreshToken.Token;
            return tokenResult;
        }

        public Account LoginAccount(LoginAccountCredentials loginCredentials)
        {
            Account user = _appDbContext.Accounts.FirstOrDefault(a => 
                (loginCredentials.EmailOrPhone == a.Email || loginCredentials.EmailOrPhone == a.Phone) 
                && a.HashPassword == loginCredentials.Password);
            return user;
        }

        public bool RegisterAccount(RegisterAccountCredentials registerCredentials)
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
            _appDbContext.Accounts.Add(user);
            _appDbContext.SaveChanges();
            return true;
        }
    }
}