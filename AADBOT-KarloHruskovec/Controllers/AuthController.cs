using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Application.Auth;
using static AADBOT_KarloHruskovec.Application.Common.Validation;

namespace AADBOT_KarloHruskovec.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IJwtService _jwtService;

		public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, IJwtService jwtService)
		{
			_authService = authService;
			_userManager = userManager;
			_jwtService = jwtService;
		}

		[HttpPost("refresh-token")]
		public IActionResult RefreshToken([FromBody] RefreshTokenRequest model)
		{
			var cmd = model.ToCommand();
			if (!_jwtService.ValidateRefreshToken(cmd.Email, cmd.RefreshToken))
				return Unauthorized(new { Message = "Invalid refresh token." });

			var user = _userManager.FindByEmailAsync(cmd.Email).Result;
			var isAdmin = cmd.Email.Contains("admin", StringComparison.OrdinalIgnoreCase);
			var newAccessToken = _jwtService.GenerateAccessToken(cmd.Email, isAdmin);
			return Ok(new { AccessToken = newAccessToken });
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest model)
		{
			var cmd = model.ToCommand();

			var errors = Validate(cmd,
				NotEmpty(c => c.Email, "Email is required."),
				EmailFormat(c => c.Email, "Email format invalid."),
				NotEmpty(c => c.Password, "Password is required.")
			);

			if (errors.Any())
				return BadRequest(errors);

			if (!new[] { "FREE", "PRO", "GOLD" }.Contains(cmd.Package))
				return BadRequest("Invalid package.");

			var (success, serviceErrors) = await _authService.RegisterAsync(cmd);
			if (!success) return BadRequest(serviceErrors);

			return Ok(new { Message = "Registration successful." });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest model)
		{
			var cmd = model.ToCommand();
			var (success, errors, isAdmin) = await _authService.LoginAsync(cmd);
			if (!success)
				return Unauthorized(new { Message = errors.FirstOrDefault() ?? "Login failed." });

			var user = _userManager.FindByEmailAsync(cmd.Email).Result;
			var isUserAdmin = cmd.Email.Contains("admin", StringComparison.OrdinalIgnoreCase);
			var accessToken = _jwtService.GenerateAccessToken(cmd.Email, isUserAdmin);
			var refreshToken = _jwtService.GenerateRefreshToken(cmd.Email);

			return Ok(new { Message = "Login successful.", AccessToken = accessToken, RefreshToken = refreshToken });
		}

		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();
			return Ok(new { Message = "Logout successful." });
		}

		[HttpGet("me")]
		[Authorize(AuthenticationSchemes = "JwtBearer")]
		public IActionResult Me()
		{
			if (!User.Identity.IsAuthenticated)
				return Ok(new { Email = (string?)null, IsAdmin = false });

			return Ok(new { Email = User.Identity.Name, IsAdmin = User.IsInRole("Admin") });
		}

		[HttpGet("public")]
		[AllowAnonymous]
		public IActionResult PublicEndpoint() => Ok("Accessible by everyone.");

		[HttpGet("user-only")]
		[Authorize(Roles = "USER,ADMIN")]
		public IActionResult UserEndpoint() => Ok("Accessible by USER and ADMIN.");

		[HttpGet("admin-only")]
		[Authorize(Roles = "ADMIN")]
		public IActionResult AdminEndpoint() => Ok("Accessible only by ADMIN.");

		[HttpGet("test")]
		public IActionResult Test() => Ok("Test OK");
	}
}
