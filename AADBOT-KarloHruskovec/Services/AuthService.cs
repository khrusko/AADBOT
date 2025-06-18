using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Factories;
using AADBOT_KarloHruskovec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AADBOT_KarloHruskovec.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ApplicationDbContext context,
			IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<(bool Success, string[] Errors)> RegisterAsync(RegisterRequest model)
		{
			var user = UserFactory.Create(model.Email, model.Package);

			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return (false, result.Errors.Select(e => e.Description).ToArray());

			await _userManager.AddToRoleAsync(user, "RegisteredUser");

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = $"User registered with package: {model.Package}",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
			return (true, Array.Empty<string>());
		}

		public async Task<(bool Success, string[] Errors, bool IsAdmin)> LoginAsync(LoginRequest model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return (false, new[] { "Invalid credentials." }, false);

			var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
			if (!result.Succeeded)
				return (false, new[] { "Invalid credentials." }, false);

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = "User logged in",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();

			var roles = await _userManager.GetRolesAsync(user);
			bool isAdmin = roles.Contains("Admin");

			return (true, Array.Empty<string>(), isAdmin);
		}


		public async Task LogoutAsync()
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext != null && httpContext.User.Identity?.IsAuthenticated == true)
			{
				var userId = _userManager.GetUserId(httpContext.User);

				_context.Logs.Add(new LogEntry
				{
					UserId = userId,
					Action = "User logged out",
					Timestamp = DateTime.UtcNow
				});

				await _context.SaveChangesAsync();
			}

			await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
		}


	}
}
