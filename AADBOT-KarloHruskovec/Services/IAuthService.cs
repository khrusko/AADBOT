using AADBOT_KarloHruskovec.Application.Auth;

namespace AADBOT_KarloHruskovec.Services;

public interface IAuthService
{
	Task<(bool success, IEnumerable<string> errors)> RegisterAsync(RegisterUserCommand command);
	Task<(bool success, IEnumerable<string> errors, bool isAdmin)> LoginAsync(LoginCommand command);
	Task LogoutAsync();
}
