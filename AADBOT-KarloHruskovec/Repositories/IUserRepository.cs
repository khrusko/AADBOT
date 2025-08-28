using AADBOT_KarloHruskovec.Application.Common;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Repositories
{
	public interface IUserRepository
	{
		Task<Option<ApplicationUser>> GetByIdAsync(string id);
		Task<Option<ApplicationUser>> GetByEmailAsync(string email);
		Task<IReadOnlyList<ApplicationUser>> GetAllAsync();
		Task<Result<ApplicationUser>> SaveAsync(ApplicationUser user);
	}
}
