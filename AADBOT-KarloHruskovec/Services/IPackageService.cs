namespace AADBOT_KarloHruskovec.Services
{
	public interface IPackageService
	{
		Task<(string package, long used, long? limit, bool canChange)> GetUserStatusAsync(string userId);
		Task<(bool success, string message)> ChangePackageAsync(string userId, string newPackage);
	}
}
