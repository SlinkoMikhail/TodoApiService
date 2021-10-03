using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApiService.Filters;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterAccountCredentials registerCredentials)
        {
            await _accountManager.RegisterAccount(registerCredentials);
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginAccountCredentials loginCredentials)
        {
            return Ok(await _accountManager.LoginAccount(loginCredentials));
        }
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh(TokenRequest token)
        {
            return Ok(await _accountManager.RefreshJWTTokens(token.RefreshToken));
        }
        [HttpPost]
        [Authorize]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogoutUserSession()
        {
            await _accountManager.LogoutSession(HttpContext.User);
            return Ok();
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("logoutall")]
        public async Task<IActionResult> LogoutUserAllSessions()
        {
            await _accountManager.LogoutAllSessions(HttpContext.User);
            return Ok();
        }
    }
}