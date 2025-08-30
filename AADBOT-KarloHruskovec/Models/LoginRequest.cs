namespace AADBOT_KarloHruskovec.Models
{
	public interface ILoginRequest
	{
		string Kind { get; }
	}

	public sealed class PasswordLoginRequest : ILoginRequest
	{
		public string Kind => "password";
		public string Username { get; init; } = string.Empty;
		public string Password { get; init; } = string.Empty;
	}

	public sealed class ApiKeyLoginRequest : ILoginRequest
	{
		public string Kind => "apikey";
		public string ApiKey { get; init; } = string.Empty;
	}
}
