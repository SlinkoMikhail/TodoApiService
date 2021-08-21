using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using TodoApiService.Models;
using TodoApiService.Models.DTO.Authentication;
using TodoApiService.Models.Options;
using Xunit;

namespace TodoApiService.Tests
{
    public class AccountManagerTests
    {
        private static JWTAuthOptions JWTOptions { get; } = new JWTAuthOptions
        {
                Issuer = "server",
                Audience = "client",
                TokenLifeTime = 300,
                Secret = "mysecretkey123456789"
        };
        private static Account Account { get; } = new Account
        {
            Id = Guid.NewGuid(),
            Email = "slinkom@gmail.com",
            HashPassword = "120278",
            Phone = "+375336096137",
            Role = Roles.Administrator
        };
        [Fact]
        public void GenerateJWTTokensTest()
        {
            //arrange
            var moq = new Mock<IOptions<JWTAuthOptions>>();
            moq.Setup(opt => opt.Value).Returns(JWTOptions);   
            IAccountManager accountManager = new AccountManager(moq.Object, null);
            //act
            TokenResult res = accountManager.GenerateJWTTokens(Account);
            //assert
            Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(res.RefreshToken));
        }

    }
}
