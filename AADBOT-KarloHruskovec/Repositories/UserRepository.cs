using AADBOT_KarloHruskovec.Application.Common;
using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.Models;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Option<ApplicationUser>> GetByIdAsync(string id)
		{
			var user = await _context.Users.FindAsync(id);
			return user is null ? Option<ApplicationUser>.None : Option<ApplicationUser>.Some(user);
		}

		public async Task<Option<ApplicationUser>> GetByEmailAsync(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
			return user is null ? Option<ApplicationUser>.None : Option<ApplicationUser>.Some(user);
		}

		public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync()
		{
			var users = await _context.Users.ToListAsync();
			return users;
		}

		public async Task<Result<ApplicationUser>> SaveAsync(ApplicationUser user)
		{
			try
			{
				_context.Users.Update(user);
				await _context.SaveChangesAsync();
				return Result<ApplicationUser>.Ok(user);
			}
			catch (Exception ex)
			{
				return Result<ApplicationUser>.Fail(ex.Message);
			}
		}
	}
}
