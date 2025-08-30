namespace AADBOT_KarloHruskovec.Application.Billing;

public interface IPackagePolicy
{
	string Name { get; }
	long DailyUploadQuotaBytes { get; }
}