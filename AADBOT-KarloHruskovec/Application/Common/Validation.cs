using AADBOT_KarloHruskovec.Application.Auth;

namespace AADBOT_KarloHruskovec.Application.Common;

public static class Validation
{
	public static IEnumerable<string> Validate<T>(T input, params Func<T, string?>[] rules) =>
		rules.Select(r => r(input)).Where(err => err is not null)!;

	public static Func<RegisterUserCommand, string?> NotEmpty(Func<RegisterUserCommand, string> pick, string msg) =>
		cmd => string.IsNullOrWhiteSpace(pick(cmd)) ? msg : null;

	public static Func<RegisterUserCommand, string?> EmailFormat(Func<RegisterUserCommand, string> pick, string msg) =>
		cmd => pick(cmd).Contains("@") ? null : msg;
}
