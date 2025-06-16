using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Services
{
	public class AdminService : IAdminService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public AdminService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task<List<object>> GetAllUsersAsync()
		{
			return await _userManager.Users
				.Select(u => new
				{
					u.Id,
					u.Email,
					u.Package,
					u.DailyUploadSize,
					u.LastPackageChange
				}).ToListAsync<object>();
		}

		public async Task<bool> UpdateUserPackageAsync(string userId, string newPackage)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return false;

			user.Package = newPackage;
			user.LastPackageChange = DateTime.UtcNow;

			_context.Logs.Add(new LogEntry
			{
				UserId = userId,
				Action = $"Admin changed package to {newPackage}",
				Timestamp = DateTime.UtcNow
			});

			await _userManager.UpdateAsync(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<object>> GetAllPhotosAsync()
		{
			return await _context.Photos
				.Include(p => p.User)
				.Select(p => new
				{
					p.Id,
					p.Description,
					p.Hashtags,
					p.FileName,
					p.UploadDate,
					Author = p.User.Email
				}).ToListAsync<object>();
		}

		public async Task<bool> DeletePhotoAsync(int id, string performedBy)
		{
			var photo = await _context.Photos.FindAsync(id);
			if (photo == null) return false;

			var path = Path.Combine("wwwroot/images", photo.FileName);
			if (File.Exists(path)) File.Delete(path);

			_context.Photos.Remove(photo);

			_context.Logs.Add(new LogEntry
			{
				UserId = performedBy,
				Action = $"Admin deleted photo {photo.Id}",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<LogEntry>> GetRecentLogsAsync(int count = 100)
		{
			return await _context.Logs
				.OrderByDescending(l => l.Timestamp)
				.Take(count)
				.ToListAsync();
		}
	}
}
