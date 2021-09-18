using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
                await _accountManager.RegisterAccount(registerCredentials);
                return StatusCode(StatusCodes.Status201Created);
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
            try
            {
                return Ok(await _accountManager.RefreshJWTTokens(token.RefreshToken));
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> LogoutUserSession()
        {
            await _accountManager.LogoutSession(HttpContext.User);
            return Ok();
        }
        [HttpPost]
        [Authorize]
        [Route("logoutall")]
        public async Task<IActionResult> LogoutUserAllSessions()
        {
            await _accountManager.LogoutAllSessions(HttpContext.User);
            return Ok();
        }
    }
}