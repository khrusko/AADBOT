using System;
using System.Collections.Generic;

namespace AADBOT_KarloHruskovec.Application.Validation;

public sealed class PackageValidator : IPackageValidator
{
	private static readonly HashSet<string> Allowed =
		new(StringComparer.OrdinalIgnoreCase) { "FREE", "PRO", "GOLD" };

	public bool IsValid(string? package) =>
		!string.IsNullOrWhiteSpace(package) && Allowed.Contains(package!);
}
