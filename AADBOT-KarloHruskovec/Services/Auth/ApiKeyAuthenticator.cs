using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT.Auth.Services
{
	public sealed class ApiKeyAuthenticator : IApiKeyAuthenticator
	{
		private readonly IApiKeyStore _keys;
		private readonly IJwtIssuer _jwt;

		public ApiKeyAuthenticator(IApiKeyStore keys, IJwtIssuer jwt)
		{
			_keys = keys;
			_jwt = jwt;
		}

		public async Task<AuthResult> AuthenticateAsync(ApiKeyLoginRequest request, CancellationToken ct = default)
		{
			var owner = await _keys.ResolveOwnerAsync(request.ApiKey, ct);
			if (owner is null) return AuthResult.Fail("Invalid API key");
			var (userId, username, role) = owner.Value;
			var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
			new Claim(ClaimTypes.NameIdentifier, userId),
			new Claim(ClaimTypes.Name, username),
			new Claim(ClaimTypes.Role, role)
		}, "ApiKey"));
			var token = _jwt.Issue(principal);
			return AuthResult.Ok(principal, token);
		}
	}

}
