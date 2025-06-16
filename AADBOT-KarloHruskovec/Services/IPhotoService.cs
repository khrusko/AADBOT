using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Services
{
	public interface IPhotoService
	{
		Task<bool> UploadPhotoAsync(ApplicationUser user, IFormFile file, string description, string hashtags, string format, int? resize);
		Task<List<object>> GetLatestPhotosAsync();
		Task<byte[]?> GetPhotoFileAsync(int id);
		Task<bool> UpdatePhotoAsync(ApplicationUser user, int id, string description, string hashtags);
		Task<List<object>> SearchPhotosAsync(string? hashtag, string? author, long? minSize, long? maxSize, DateTime? from, DateTime? to);
		Task<byte[]?> GetFilteredPhotoAsync(int id, DownloadFilterOptions filters);

	}
}
