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
        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterAccountCredentials registerCredentials)
        {
            if(!ModelState.IsValid) return BadRequest();
            //_IAccountManager.Register(registerCredentials);
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginAccountCredentials loginCredentials)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}