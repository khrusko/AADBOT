using System.Security.Claims;

namespace AADBOT_KarloHruskovec.Models
{
	public sealed class AuthResult
	{
		public bool Success { get; init; }
		public ClaimsPrincipal? Principal { get; init; }
		public string? Token { get; init; }
		public string? Error { get; init; }

		public static AuthResult Ok(ClaimsPrincipal principal, string token) =>
			new AuthResult { Success = true, Principal = principal, Token = token };

		public static AuthResult Fail(string error) =>
			new AuthResult { Success = false, Error = error };
	}
}
