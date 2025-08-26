using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AADBOT_KarloHruskovec.Services
{
	public class JwtTokenService : IJwtService
	{
		private readonly IConfiguration _config;
		private static readonly Dictionary<string, string> RefreshTokens = new();

		public JwtTokenService(IConfiguration config)
		{
			_config = config;
		}

		public string GenerateAccessToken(string email, bool isAdmin)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, email),
				new Claim(ClaimTypes.NameIdentifier, email),
				new Claim(ClaimTypes.Role, isAdmin ? "ADMIN" : "USER")
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!)),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public string GenerateRefreshToken(string email)
		{
			var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			RefreshTokens[email] = refreshToken;
			return refreshToken;
		}

		public bool ValidateRefreshToken(string email, string token)
		{
			return RefreshTokens.TryGetValue(email, out var stored) && stored == token;
		}
	}
}
