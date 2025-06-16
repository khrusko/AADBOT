using AADBOT_KarloHruskovec.DTOs;

namespace AADBOT_KarloHruskovec.Services
{
	public interface IAuthService
	{
		Task<(bool Success, string[] Errors)> RegisterAsync(RegisterRequest model);
		Task<(bool Success, string[] Errors)> LoginAsync(LoginRequest model);
		Task LogoutAsync();
	}
}
