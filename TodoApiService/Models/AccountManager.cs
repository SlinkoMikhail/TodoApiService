using System;
using System.IdentityModel.Tokens.Jwt;
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