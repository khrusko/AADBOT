using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace AADBOT.Auth.Services
{
	public interface IPasswordVerifier
	{
		Task<bool> VerifyAsync(string username, string password, CancellationToken ct = default);
		ClaimsPrincipal BuildPrincipal(string username);
	}

	public interface IApiKeyStore
	{
		Task<(string UserId, string Username, string Role)?> ResolveOwnerAsync(string apiKey, CancellationToken ct = default);
	}

	public interface IJwtIssuer
	{
		string Issue(ClaimsPrincipal principal);
	}
}
