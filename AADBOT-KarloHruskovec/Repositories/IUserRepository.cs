using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Repositories
{
	public interface IUserRepository
	{
		Task<ApplicationUser?> GetByIdAsync(string id);
		Task<ApplicationUser?> GetByEmailAsync(string email);
		Task<List<ApplicationUser>> GetAllAsync();
		Task SaveAsync(ApplicationUser user);
	}

}
