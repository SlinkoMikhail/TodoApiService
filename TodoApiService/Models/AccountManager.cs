using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApiService.Extensions;
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
        private async Task<TokenResult> GenerateJWTTokens(Session session)
        {
            if(session == null) throw new ArgumentNullException("Session can't be null.");
            //access_token
            TokenResult tokenResult = new TokenResult();
            SigningCredentials signingCredentials = new SigningCredentials(_jwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);
            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimNames.AccountId, session.AccountId.ToString()),
                new Claim(ClaimNames.SessionId, session.Id.ToString())
            };
            JwtSecurityToken accessJWTToken = new JwtSecurityToken(
                issuer: _jwtAuthOptions.Issuer,
                claims: claims,
                audience: _jwtAuthOptions.Audience,
                expires: DateTime.UtcNow.AddSeconds(_jwtAuthOptions.AccessTokenLifeTimeSeconds),
                signingCredentials: signingCredentials
            );
            tokenResult.AccessToken = jwtHandler.WriteToken(accessJWTToken);
            //refresh_token 
            RefreshToken refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenLifeTimeDays),
                Session = session,
                SessionId = session.Id
            };
            RefreshToken refreshTokenStored = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Session.Id == session.Id);
            if(refreshTokenStored == null)
            {
                await _appDbContext.RefreshTokens.AddAsync(refreshToken);
            }
            else
            {
                refreshTokenStored.AddedDate = refreshToken.AddedDate;
                refreshTokenStored.ExpiryDate = refreshToken.ExpiryDate;
                refreshTokenStored.IsRevoked = refreshToken.IsRevoked;
                refreshTokenStored.Token = refreshToken.Token;
            }
            await _appDbContext.SaveChangesAsync();
            var refreshJWTToken = new JwtSecurityToken(
                issuer: _jwtAuthOptions.Issuer,
                claims: claims.Append(new Claim(ClaimNames.RefreshBase, refreshToken.Token)),
                audience: _jwtAuthOptions.Audience,
                expires: refreshToken.ExpiryDate,
                signingCredentials: signingCredentials
            );
            tokenResult.RefreshToken = jwtHandler.WriteToken(refreshJWTToken);
            return tokenResult;
        }
        public Account GetAccountById(Guid id) => _appDbContext.Accounts.Find(id);
        public async Task<Account> GetAccountByIdAsync(Guid id) => await _appDbContext.Accounts.FindAsync(id);
        public async Task<TokenResult> LoginAccount(LoginAccountCredentials loginCredentials)
        {
            Account user = await _appDbContext.Accounts.FirstOrDefaultAsync(a => 
                (loginCredentials.EmailOrPhone.Trim().ToLowerInvariant() == a.Email 
                || loginCredentials.EmailOrPhone.Trim().ToLowerInvariant() == a.Phone));
            if(user == null || !BCrypt.Net.BCrypt.Verify(loginCredentials.Password, user.HashPassword))
                throw new SecurityTokenException("Incorrect password or this email is not registered.");
            Session session = new Session
            {
                Account = user,
                AccountId = user.Id,
            };
            await _appDbContext.Sessions.AddAsync(session);
            await _appDbContext.SaveChangesAsync();
            return await GenerateJWTTokens(session);
        }
        public async Task<TokenResult> RefreshJWTTokens(string refreshToken)
        {
            var (accountId, sessionId, refresh) = ValidateAndExtractValuesFromRefreshToken(refreshToken);
            RefreshToken refreshTokenStored = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refresh.ToString());
            if(refreshTokenStored == null || refreshTokenStored.IsRevoked || refreshTokenStored.SessionId != sessionId)
            {
                throw new SecurityTokenException("Token is invalid.");
            }
            Session session = await _appDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            if(session == null || session.AccountId != accountId)
            {
                throw new SecurityTokenException("Token is invalid");
            }
            return await this.GenerateJWTTokens(session);
        }
        private (Guid accountId, Guid sessionId, Guid refresh) ValidateAndExtractValuesFromRefreshToken(string refreshToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validations = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtAuthOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtAuthOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtAuthOptions.GetSymmetricSecurityKey(),
                ValidateLifetime = true,
                ClockSkew = _jwtAuthOptions.ClockSkew
            };
            ClaimsPrincipal claims = handler.ValidateToken(refreshToken, validations, out _);
            Guid refresh = claims.GetRefresh();
            Guid accoutId = claims.GetAccountId();
            Guid sessionId = claims.GetSessionId();
            if(refresh == default || accoutId == default || sessionId == default)
            {
                throw new SecurityTokenException("Token hasn't got required claims");
            }
            return(accoutId, sessionId, refresh);
        }

        public async Task RegisterAccount(RegisterAccountCredentials registerCredentials)
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
        }

        public async Task LogoutSession(ClaimsPrincipal claims)
        {
            if(claims == null) throw new ArgumentNullException("Claims can't be null");
            Guid sessionId = claims.GetSessionId();
            Session session = await _appDbContext.Sessions.FindAsync(sessionId);
            if(session.AccountId == claims.GetAccountId())
            {
                _appDbContext.Sessions.Remove(session);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new SecurityTokenException("User session not found."); 
            }
        }

        public async Task LogoutAllSessions(ClaimsPrincipal claims)
        {
            if(claims == null) throw new ArgumentNullException("Claims can't be null");
            Guid userId = claims.GetAccountId();
            if(userId != default)
            {
                Session[] userSessions = await _appDbContext.Sessions.Where(s => s.AccountId == userId).ToArrayAsync();
                _appDbContext.Sessions.RemoveRange(userSessions);
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                throw new SecurityTokenException("User sessions not found.");
            }
        }
    }
}