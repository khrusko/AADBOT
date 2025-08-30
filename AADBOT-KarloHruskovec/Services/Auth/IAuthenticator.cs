using System.Threading;
using System.Threading.Tasks;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT.Auth.Services
{
	public interface IAuthenticator<TRequest> where TRequest : ILoginRequest
	{
		Task<AuthResult> AuthenticateAsync(TRequest request, CancellationToken ct = default);
	}

	//Interface segregation principle
	public interface IPasswordAuthenticator : IAuthenticator<PasswordLoginRequest> { }

	public interface IApiKeyAuthenticator : IAuthenticator<ApiKeyLoginRequest> { }
}
