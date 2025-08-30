namespace AADBOT_KarloHruskovec.Application.Billing;

public sealed class FreePolicy : IPackagePolicy
{
	public string Name => "FREE";
	public long DailyUploadQuotaBytes => 50 * 1024 * 1024;
}

public sealed class ProPolicy : IPackagePolicy
{
	public string Name => "PRO";
	public long DailyUploadQuotaBytes => 500 * 1024 * 1024;
}

public sealed class GoldPolicy : IPackagePolicy
{
	public string Name => "GOLD";
	public long DailyUploadQuotaBytes => 2_000 * 1024 * 1024;
}
