using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AADBOT.Auth.Services;
using AADBOT_KarloHruskovec.Options;
using Microsoft.Extensions.Options;

namespace AADBOT.Infrastructure.Auth
{
	public sealed class ApiKeyStore : IApiKeyStore
	{
		private readonly AuthSeeds _seeds;

		public ApiKeyStore(IOptions<AuthSeeds> seeds)
		{
			_seeds = seeds.Value;
		}

		public Task<(string UserId, string Username, string Role)?> ResolveOwnerAsync(string apiKey, CancellationToken ct = default)
		{
			var k = _seeds.ApiKeys.FirstOrDefault(x => x.Key == apiKey);
			if (k is null)
				return Task.FromResult<(string, string, string)?>(null);

			return Task.FromResult<(string, string, string)?>((k.UserId, k.Username, k.Role));
		}

	}
}
