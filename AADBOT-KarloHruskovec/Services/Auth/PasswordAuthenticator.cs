using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT.Auth.Services
{
	public sealed class PasswordAuthenticator : IPasswordAuthenticator
	{
		private readonly IPasswordVerifier _verifier;
		private readonly IJwtIssuer _jwt;

		public PasswordAuthenticator(IPasswordVerifier verifier, IJwtIssuer jwt)
		{
			_verifier = verifier;
			_jwt = jwt;
		}

		public async Task<AuthResult> AuthenticateAsync(PasswordLoginRequest request, CancellationToken ct = default)
		{
			var ok = await _verifier.VerifyAsync(request.Username, request.Password, ct);
			if (!ok) return AuthResult.Fail("Invalid credentials");
			var principal = _verifier.BuildPrincipal(request.Username);
			var token = _jwt.Issue(principal);
			return AuthResult.Ok(principal, token);
		}
	}

}
