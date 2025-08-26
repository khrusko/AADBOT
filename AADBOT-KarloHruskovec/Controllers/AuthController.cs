using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AADBOT_KarloHruskovec.Models;

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
			if (!_jwtService.ValidateRefreshToken(model.Email, model.RefreshToken))
				return Unauthorized(new { Message = "Invalid refresh token." });

			var user = _userManager.FindByEmailAsync(model.Email).Result;
			var isAdmin = model.Email.Contains("admin", StringComparison.OrdinalIgnoreCase);
			var newAccessToken = _jwtService.GenerateAccessToken(model.Email, isAdmin);


			return Ok(new { AccessToken = newAccessToken });
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
			var (success, errors, isAdmin) = await _authService.LoginAsync(model);
			if (!success)
				return Unauthorized(new { Message = errors.FirstOrDefault() ?? "Login failed." });

			var user = _userManager.FindByEmailAsync(model.Email).Result;
			var isUserAdmin = model.Email.Contains("admin", StringComparison.OrdinalIgnoreCase);
			var accessToken = _jwtService.GenerateAccessToken(model.Email, isUserAdmin);
			var refreshToken = _jwtService.GenerateRefreshToken(model.Email);

			return Ok(new
			{
				Message = "Login successful.",
				AccessToken = accessToken,
				RefreshToken = refreshToken
			});

		}


		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();
			return Ok(new { Message = "Logout successful." });
		}

		[HttpGet("me")]
		public IActionResult Me()
		{
			if (!User.Identity.IsAuthenticated)
				return Ok(new { Email = (string?)null, IsAdmin = false });

			return Ok(new
			{
				Email = User.Identity.Name,
				IsAdmin = User.IsInRole("Admin")
			});
		}

		[HttpGet("public")]
		[AllowAnonymous]
		public IActionResult PublicEndpoint()
		{
			return Ok("Accessible by everyone.");
		}

		[HttpGet("user-only")]
		[Authorize(Roles = "USER,ADMIN")]
		public IActionResult UserEndpoint()
		{
			return Ok("Accessible by USER and ADMIN.");
		}

		[HttpGet("admin-only")]
		[Authorize(Roles = "ADMIN")]
		public IActionResult AdminEndpoint()
		{
			return Ok("Accessible only by ADMIN.");
		}

		[HttpGet("test")]
		public IActionResult Test()
		{
			return Ok("Test OK");
		}


	}
}
