using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Services
{
	public class PackageService : IPackageService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public PackageService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<(string package, long used, long? limit, bool canChange)> GetUserStatusAsync(string userId)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null) throw new Exception("User not found");

			if (user.LastUploadReset?.Date != DateTime.UtcNow.Date)
			{
				user.DailyUploadSize = 0;
				user.LastUploadReset = DateTime.UtcNow;
				await _userManager.UpdateAsync(user);
			}

			long? limit = user.Package switch
			{
				"FREE" => 10 * 1024 * 1024,
				"PRO" => 100 * 1024 * 1024,
				"GOLD" => null,
				_ => null
			};

			bool canChange = !user.LastPackageChange.HasValue || user.LastPackageChange.Value.Date < DateTime.UtcNow.Date;

			return (user.Package, user.DailyUploadSize, limit, canChange);
		}

		public async Task<(bool success, string message)> ChangePackageAsync(string userId, string newPackage)
		{
			if (!new[] { "FREE", "PRO", "GOLD" }.Contains(newPackage))
				return (false, "Invalid package");

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return (false, "User not found");

			if (user.LastPackageChange?.Date == DateTime.UtcNow.Date)
				return (false, "Package can only be changed once per day");

			user.Package = newPackage;
			user.LastPackageChange = DateTime.UtcNow;

			_context.Logs.Add(new LogEntry
			{
				UserId = userId,
				Action = $"Changed package to {newPackage}",
				Timestamp = DateTime.UtcNow
			});

			await _userManager.UpdateAsync(user);
			await _context.SaveChangesAsync();

			return (true, "Package changed.");
		}
	}
}
