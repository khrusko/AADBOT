using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Services
{
	public class PhotoService : IPhotoService
	{
		private readonly ApplicationDbContext _context;
		private readonly IImageService _imageService;

		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<ApplicationUser> _userManager;

		public PhotoService(
			ApplicationDbContext context,
			IImageService imageService,
			IHttpContextAccessor httpContextAccessor,
			UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_imageService = imageService;
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;

		}


		public async Task<bool> UploadPhotoAsync(ApplicationUser user, IFormFile file, string description, string hashtags, string format, int? resize)
		{
			var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

			var (bytes, newFileName, fmt) = await _imageService.ProcessImageAsync(file, format, resize);
			var path = Path.Combine("wwwroot/images", newFileName);
			await File.WriteAllBytesAsync(path, bytes);

			var photo = new Photo
			{
				FileName = newFileName,
				Format = fmt,
				Size = bytes.Length,
				Description = description,
				Hashtags = hashtags,
				UploadDate = DateTime.UtcNow,
				UserId = user.Id
			};

			if (!isAdmin)
				user.DailyUploadSize += bytes.Length;

			_context.Photos.Add(photo);

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = $"Uploaded photo {newFileName}",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
			return true;
		}


		public async Task<List<object>> GetLatestPhotosAsync()
		{
			return await _context.Photos
				.Include(p => p.User)
				.OrderByDescending(p => p.UploadDate)
				//.Take(10)
				.Select(p => new
				{
					p.Id,
					p.FileName,
					p.Description,
					p.Hashtags,
					p.UploadDate,
					Author = p.User.Email
				}).ToListAsync<object>();
		}

		public async Task<byte[]?> GetPhotoFileAsync(int id)
		{
			var photo = await _context.Photos.FindAsync(id);
			if (photo == null) return null;

			var path = Path.Combine("wwwroot/images", photo.FileName);
			if (!File.Exists(path)) return null;

			return await File.ReadAllBytesAsync(path);
		}

		public async Task<bool> UpdatePhotoAsync(ApplicationUser user, int id, string description, string hashtags)
		{
			var photo = await _context.Photos.FindAsync(id);
			if (photo == null)
				return false;

			var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
			if (photo.UserId != user.Id && !isAdmin)
				return false;

			photo.Description = description;
			photo.Hashtags = hashtags;

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = $"Edited photo {photo.Id}",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<object>> SearchPhotosAsync(string? hashtag, string? author, long? minSize, long? maxSize, DateTime? from, DateTime? to)
		{
			var query = _context.Photos.Include(p => p.User).AsQueryable();

			if (!string.IsNullOrEmpty(hashtag)) query = query.Where(p => p.Hashtags.Contains(hashtag));
			if (!string.IsNullOrEmpty(author)) query = query.Where(p => p.User.Email.Contains(author));
			if (minSize.HasValue) query = query.Where(p => p.Size >= minSize);
			if (maxSize.HasValue) query = query.Where(p => p.Size <= maxSize);
			if (from.HasValue) query = query.Where(p => p.UploadDate >= from);
			if (to.HasValue) query = query.Where(p => p.UploadDate <= to);

			return await query.Select(p => new
			{
				p.Id,
				p.FileName,
				p.Description,
				p.Hashtags,
				p.UploadDate,
				Author = p.User.Email
			}).ToListAsync<object>();
		}
		public async Task<byte[]?> GetFilteredPhotoAsync(int id, DownloadFilterOptions filters)
		{
			var photo = await _context.Photos.FindAsync(id);
			if (photo == null) return null;

			var path = Path.Combine("wwwroot/images", photo.FileName);
			if (!File.Exists(path)) return null;

			var originalBytes = await File.ReadAllBytesAsync(path);
			var processed = await _imageService.ApplyFiltersAsync(originalBytes, filters);
			string? userId = null;
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext?.User.Identity?.IsAuthenticated == true)
			{
				userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			}

			_context.Logs.Add(new LogEntry
			{
				UserId = userId,
				Action = $"Downloaded photo ID:{photo.Id}",
				Timestamp = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();

			return processed;
		}



	}
}
