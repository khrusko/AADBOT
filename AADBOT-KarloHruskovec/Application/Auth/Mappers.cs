using AADBOT_KarloHruskovec.DTOs;

namespace AADBOT_KarloHruskovec.Application.Auth;

public static class Mappers
{
	public static RegisterUserCommand ToCommand(this RegisterRequest x) =>
		new(x.Email.Trim(), x.Password, x.Package.Trim());

	public static LoginCommand ToCommand(this LoginRequest x) =>
		new(x.Email.Trim(), x.Password);

	public static RefreshTokenCommand ToCommand(this RefreshTokenRequest x) =>
		new(x.Email.Trim(), x.RefreshToken);
}
