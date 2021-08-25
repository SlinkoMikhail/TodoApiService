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
            RefreshToken refreshTokenStored = _appDbContext.RefreshTokens.FirstOrDefault(t => t.AccountId == refreshToken.AccountId);
            if(refreshTokenStored == null)
            { 
                _appDbContext.RefreshTokens.Add(refreshToken);
            }
            else
            {
                refreshTokenStored.Account = refreshToken.Account;
                refreshTokenStored.AccountId = refreshToken.AccountId;
                refreshTokenStored.AddedDate = refreshToken.AddedDate;
                refreshTokenStored.ExpiryDate = refreshToken.ExpiryDate;
                refreshTokenStored.IsRevoked = refreshToken.IsRevoked;
                refreshTokenStored.IsUsed = refreshToken.IsUsed;
                refreshTokenStored.Token = refreshToken.Token;
                _appDbContext.RefreshTokens.Update(refreshTokenStored);
            }
            _appDbContext.SaveChanges();
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

        public TokenResult RefreshJWTTokens(string refreshToken)
        {
            RefreshToken refresh = _appDbContext.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if(refresh != null && refresh.ExpiryDate > DateTime.UtcNow && refresh.IsRevoked == false && refresh.IsUsed == false)
            {
                refresh.IsUsed = true;
                Account user = _appDbContext.Accounts.Find(refresh.AccountId);
                TokenResult tokenResult = this.GenerateJWTTokens(user);
                if(tokenResult != null)
                    return tokenResult;
            }
            return null;
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