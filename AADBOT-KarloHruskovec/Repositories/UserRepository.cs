using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Repositorires
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ApplicationUser?> GetByIdAsync(string id) =>
			await _context.Users.FindAsync(id);

		public async Task<ApplicationUser?> GetByEmailAsync(string email) =>
			await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

		public async Task<List<ApplicationUser>> GetAllAsync() =>
			await _context.Users.ToListAsync();

		public async Task SaveAsync(ApplicationUser user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}
	}

}
