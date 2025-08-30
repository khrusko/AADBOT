using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AADBOT.Auth.Services;
using AADBOT_KarloHruskovec.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AADBOT.Infrastructure.Auth
{
	public sealed class PasswordVerifier : IPasswordVerifier
	{
		private readonly AuthSeeds _seeds;
		private readonly PasswordHasher<string> _hasher = new PasswordHasher<string>();

		public PasswordVerifier(IOptions<AuthSeeds> seeds)
		{
			_seeds = seeds.Value;
		}

		public Task<bool> VerifyAsync(string username, string password, CancellationToken ct = default)
		{
			var u = _seeds.Users.FirstOrDefault(x => x.Username == username);
			if (u is null) return Task.FromResult(false);
			var res = _hasher.VerifyHashedPassword(u.Username, u.PasswordHash, password);
			return Task.FromResult(res == PasswordVerificationResult.Success || res == PasswordVerificationResult.SuccessRehashNeeded);
		}

		public ClaimsPrincipal BuildPrincipal(string username)
		{
			var u = _seeds.Users.First(x => x.Username == username);
			var identity = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.NameIdentifier, u.UserId),
				new Claim(ClaimTypes.Name, u.Username),
				new Claim(ClaimTypes.Role, u.Role)
			}, "Password");
			return new ClaimsPrincipal(identity);
		}
	}
}
