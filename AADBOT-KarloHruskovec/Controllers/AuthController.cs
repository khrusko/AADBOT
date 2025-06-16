using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AADBOT_KarloHruskovec.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			if (!new[] { "FREE", "PRO", "GOLD" }.Contains(model.Package))
				return BadRequest("Invalid package.");

			var (success, errors) = await _authService.RegisterAsync(model);
			if (!success)
				return BadRequest(errors);

			return Ok(new { Message = "Registration successful." });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest model)
		{
			var (success, errors) = await _authService.LoginAsync(model);
			if (!success)
				return Unauthorized(new { Message = errors.FirstOrDefault() ?? "Login failed." });

			return Ok(new { Message = "Login successful." });
		}

		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();
			return Ok(new { Message = "Logout successful." });
		}
	}
}
