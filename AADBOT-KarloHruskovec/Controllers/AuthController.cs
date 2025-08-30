using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Application.Auth;
using static AADBOT_KarloHruskovec.Application.Common.Validation;
using AADBOT_KarloHruskovec.Application.Validation;
using AADBOT.Auth.Services;

namespace AADBOT_KarloHruskovec.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IJwtService _jwtService;
		private readonly IPackageValidator _packageValidator;

		public AuthController(
			IAuthService authService,
			UserManager<ApplicationUser> userManager,
			IJwtService jwtService,
			IPackageValidator packageValidator)
		{
			_authService = authService;
			_userManager = userManager;
			_jwtService = jwtService;
			_packageValidator = packageValidator;
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

			if (!_packageValidator.IsValid(cmd.Package))
				return BadRequest("Invalid package.");

			var (success, errors) = await _authService.RegisterAsync(cmd);
			if (!success) return BadRequest(errors);

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

		[HttpPost("login/password")]
		public async Task<ActionResult<AuthResult>> Password([FromBody] PasswordLoginRequest request, CancellationToken ct,
	[FromServices] IPasswordAuthenticator auth)
		{
			var result = await auth.AuthenticateAsync(request, ct);
			return result.Success ? Ok(result) : Unauthorized(result);
		}

		[HttpPost("login/apikey")]
		public async Task<ActionResult<AuthResult>> ApiKey([FromBody] ApiKeyLoginRequest request, CancellationToken ct,
			[FromServices] IApiKeyAuthenticator auth)
		{
			var result = await auth.AuthenticateAsync(request, ct);
			return result.Success ? Ok(result) : Unauthorized(result);
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
