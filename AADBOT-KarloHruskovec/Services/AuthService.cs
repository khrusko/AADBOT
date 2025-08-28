using System.Linq;
using System.Threading.Tasks;
using AADBOT_KarloHruskovec.Application.Auth;
using AADBOT_KarloHruskovec.Application.Common;
using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.Factories;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using AADBOT_KarloHruskovec.Application.Security;
using System;

namespace AADBOT_KarloHruskovec.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUserRepository _userRepository;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ApplicationDbContext context,
			IHttpContextAccessor httpContextAccessor,
			IUserRepository userRepository)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_userRepository = userRepository;
		}

		public async Task<(bool success, IEnumerable<string> errors)> RegisterAsync(RegisterUserCommand command)
		{
			// functional normalization + package stamping
			var user = UserFactory.Create(command.Email, command.Package)
								  .WithEmail(command.Email.Trim().ToLowerInvariant())
								  .WithPackage(command.Package);

			var result = await _userManager.CreateAsync(user, command.Password);
			if (!result.Succeeded)
				return (false, result.Errors.Select(e => e.Description));

			await _userManager.AddToRoleAsync(user, "RegisteredUser");

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = $"User registered with package: {command.Package}",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
			return (true, Array.Empty<string>());
		}

		public async Task<(bool success, IEnumerable<string> errors, bool isAdmin)> LoginAsync(LoginCommand command)
		{
			var maybeUser = await _userRepository.GetByEmailAsync(command.Email);
			if (!maybeUser.HasValue)
				return (false, new[] { "Invalid credentials." }, false);

			var user = maybeUser.Value;

			var result = await _signInManager.PasswordSignInAsync(user, command.Password, false, false);
			if (!result.Succeeded)
				return (false, new[] { "Invalid credentials." }, false);

			// functional daily reset
			if (user.LastUploadReset == null || user.LastUploadReset.Value.Date < DateTime.UtcNow.Date)
			{
				var resetUser = user.WithDailyUploadSize(0);
				await _userRepository.SaveAsync(resetUser);
			}

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = "User logged in",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();

			var roles = await _userManager.GetRolesAsync(user);
			var isAdmin = roles.Contains("Admin");

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
				await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
			}
		}
	}
}
