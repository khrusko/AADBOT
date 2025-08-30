namespace AADBOT_KarloHruskovec.Options
{
	public sealed class SeedUser
	{
		public string UserId { get; init; } = string.Empty;
		public string Username { get; init; } = string.Empty;
		public string PasswordHash { get; init; } = string.Empty;
		public string Role { get; init; } = "User";
	}

	public sealed class SeedApiKey
	{
		public string Key { get; init; } = string.Empty;
		public string UserId { get; init; } = string.Empty;
		public string Username { get; init; } = string.Empty;
		public string Role { get; init; } = "Service";
	}

	public sealed class AuthSeeds
	{
		public SeedUser[] Users { get; init; } = new SeedUser[0];
		public SeedApiKey[] ApiKeys { get; init; } = new SeedApiKey[0];
	}
}
