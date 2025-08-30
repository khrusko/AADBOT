using System;
using System.Collections.Generic;
using System.Linq;

namespace AADBOT_KarloHruskovec.Application.Billing;

public interface IPackagePolicyResolver
{
	IPackagePolicy Resolve(string package);
}

public sealed class PackagePolicyResolver : IPackagePolicyResolver
{
	private readonly IReadOnlyDictionary<string, IPackagePolicy> _map;

	public PackagePolicyResolver(IEnumerable<IPackagePolicy> policies)
	{
		_map = policies.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
	}

	public IPackagePolicy Resolve(string package)
	{
		if (_map.TryGetValue(package, out var policy))
			return policy;

		throw new KeyNotFoundException($"Unknown package '{package}'.");
	}
}
