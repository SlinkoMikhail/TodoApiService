using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<IActionResult> Register(RegisterAccountCredentials registerCredentials)
        {
            try
            {
                return Ok(await _accountManager.RegisterAccount(registerCredentials));
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginAccountCredentials loginCredentials)
        {
            try
            {
                return Ok(await _accountManager.LoginAccount(loginCredentials));
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(TokenRequest token)
        {
            TokenResult result = await _accountManager.RefreshJWTTokens(token.RefreshToken);
            if(result == null) return BadRequest();
            return Ok(result);
        }
    }
}