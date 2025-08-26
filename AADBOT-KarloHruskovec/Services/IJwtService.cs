namespace AADBOT_KarloHruskovec.Services
{
	public interface IJwtService
	{
		public string GenerateAccessToken(string email, bool isAdmin);
		public string GenerateRefreshToken(string email);
		public bool ValidateRefreshToken(string email, string token);
	}
}
