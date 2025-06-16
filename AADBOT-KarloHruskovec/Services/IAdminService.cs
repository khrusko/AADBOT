using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Services
{
	public interface IAdminService
	{
		Task<List<object>> GetAllUsersAsync();
		Task<bool> UpdateUserPackageAsync(string userId, string newPackage);
		Task<List<object>> GetAllPhotosAsync();
		Task<bool> DeletePhotoAsync(int id, string performedBy);
		Task<List<LogEntry>> GetRecentLogsAsync(int count = 100);
	}
}
