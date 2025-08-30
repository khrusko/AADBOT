using System.Threading;
using System.Threading.Tasks;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT.Auth.Services
{
	public sealed class AuthenticatorPipeline
	{
		private readonly IPasswordAuthenticator _password;
		private readonly IApiKeyAuthenticator _apikey;

		public AuthenticatorPipeline(IPasswordAuthenticator password, IApiKeyAuthenticator apikey)
		{
			_password = password;
			_apikey = apikey;
		}

		public Task<AuthResult> AuthenticateAsync(ILoginRequest request, CancellationToken ct = default)
		{
			return request switch
			{
				PasswordLoginRequest r => _password.AuthenticateAsync(r, ct),
				ApiKeyLoginRequest r => _apikey.AuthenticateAsync(r, ct),
				_ => Task.FromResult(AuthResult.Fail("Unsupported login kind"))
			};
		}
	}
}
