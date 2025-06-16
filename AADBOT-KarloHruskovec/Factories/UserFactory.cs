using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Factories
{
	public static class UserFactory
	{
		public static ApplicationUser Create(string email, string package)
		{
			return new ApplicationUser
			{
				UserName = email,
				Email = email,
				Package = package,
				LastPackageChange = DateTime.UtcNow
			};
		}
	}
}
