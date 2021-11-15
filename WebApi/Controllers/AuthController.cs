using System.Threading.Tasks;
using Core.Dto.Auth.Request;
using Core.Dto.Auth.Response;
using Core.Entity;
using Core.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            return Ok(await _authService.LoginAsync(request));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(SignUpRequest request)
        {
            return Ok(await _authService.RegisterAsync(request));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _authService.ForgotPassword(model);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            return Ok(await _authService.ResetPassword(model));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePwdRequest model)
        {

            return Ok(await _authService.ChangePassword(model));
        }
    }
}
