using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AADBOT.Auth.Services;
using AADBOT_KarloHruskovec.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AADBOT.Infrastructure
{
	public sealed class JwtIssuer : IJwtIssuer
	{
		private readonly JwtOptions _opts;

		public JwtIssuer(IOptions<JwtOptions> opts)
		{
			_opts = opts.Value;
		}

		public string Issue(ClaimsPrincipal principal)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var jwt = new JwtSecurityToken(_opts.Issuer, _opts.Audience, principal.Claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(_opts.ExpiresMinutes), creds);
			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}
	}
}
