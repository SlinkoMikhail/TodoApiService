using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApiService.Models.DTO.Authentication;

namespace TodoApiService.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        [Route("register")]
        public IActionResult Register(AccountCredentials accountCredentials)
        {
            throw new NotImplementedException();
        }
        [Route("login")]
        public async Task<IActionResult> Login(AccountCredentials accountCredentials)
        {
            throw new NotImplementedException();
        }
        [Route("refresh")]
        public async Task<IActionResult> Refresh(string RefreshToken)
        {
            throw new NotImplementedException();
        }
    }
}