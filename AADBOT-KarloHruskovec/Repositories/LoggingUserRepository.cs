using AADBOT_KarloHruskovec.Application.Common;
using AADBOT_KarloHruskovec.Models;
using Microsoft.Extensions.Logging;

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

		public async Task<Option<ApplicationUser>> GetByIdAsync(string id)
		{
			_logger.LogInformation("GetByIdAsync called with {Id}", id);
			return await _inner.GetByIdAsync(id);
		}

		public async Task<Option<ApplicationUser>> GetByEmailAsync(string email)
		{
			_logger.LogInformation("GetByEmailAsync called with {Email}", email);
			return await _inner.GetByEmailAsync(email);
		}

		public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
		{
			_logger.LogInformation("GetAllAsync called");
			return await _inner.GetAllAsync();
		}

		public async Task<Result<ApplicationUser>> SaveAsync(ApplicationUser user)
		{
			_logger.LogInformation("SaveAsync called for {Email}", user.Email);
			return await _inner.SaveAsync(user);
		}
	}
}
