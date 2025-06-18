using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Repositories
{
	public class LoggingUserRepository : IUserRepository
	{
		private readonly IUserRepository _inner;
		private readonly ILogger<LoggingUserRepository> _logger;

		public LoggingUserRepository(IUserRepository inner, ILogger<LoggingUserRepository> logger)
		{
			_inner = inner;
			_logger = logger;
		}

		public async Task<ApplicationUser?> GetByIdAsync(string id)
		{
			_logger.LogInformation("Fetching user by ID: {Id}", id);
			return await _inner.GetByIdAsync(id);
		}

		public async Task<ApplicationUser?> GetByEmailAsync(string email)
		{
			_logger.LogInformation("Fetching user by Email: {Email}", email);
			return await _inner.GetByEmailAsync(email);
		}

		public async Task<List<ApplicationUser>> GetAllAsync()
		{
			_logger.LogInformation("Fetching all users");
			return await _inner.GetAllAsync();
		}

		public async Task SaveAsync(ApplicationUser user)
		{
			_logger.LogInformation("Saving user: {Email}", user.Email);
			await _inner.SaveAsync(user);
		}
	}

}
