using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApiService.Models;
using TodoApiService.Models.DTO.Authentication;

namespace TodoApiService.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        public AuthenticationController(IAccountManager accountManager)
        {
            _accountManager = accountManager;   
        }
        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterAccountCredentials registerCredentials)
        {
            if(!ModelState.IsValid) return BadRequest();
            if(_accountManager.RegisterAccount(registerCredentials))
                return Ok();
            return BadRequest();
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginAccountCredentials loginCredentials)
        {
            Account user = _accountManager.LoginAccount(loginCredentials);
            if(user == null) return BadRequest();
            TokenResult result = _accountManager.GenerateJWTTokens(user);
            if(result != null)
                return Ok(result);
            return BadRequest();
        }
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenRequest token)
        {
            TokenResult result = _accountManager.RefreshJWTTokens(token.RefreshToken);
            if(result == null) return BadRequest();
            return Ok(result);
        }
    }
}