using System.Security.Cryptography;
using System.Text;

namespace AADBOT_KarloHruskovec.Application.Security;

public static class PasswordHashing
{
	public static Func<string, string> Normalize => s => s.Trim();

	public static Func<string, string> Salt(string salt) =>
		s => s + salt;

	public static Func<string, string> Sha256 => s =>
	{
		using var sha = SHA256.Create();
		var bytes = Encoding.UTF8.GetBytes(s);
		var hash = sha.ComputeHash(bytes);
		return Convert.ToHexString(hash);
	};

	public static Func<string, string> Pipe(params Func<string, string>[] steps) =>
		input => steps.Aggregate(input, (acc, f) => f(acc));
}
