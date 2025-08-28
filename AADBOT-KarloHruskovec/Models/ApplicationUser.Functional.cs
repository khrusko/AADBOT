using System;

namespace AADBOT_KarloHruskovec.Models
{
	public partial class ApplicationUser
	{
		public ApplicationUser WithEmail(string email)
		{
			var copy = (ApplicationUser)MemberwiseClone();
			copy.Email = email;
			return copy;
		}

		public ApplicationUser WithPackage(string package)
		{
			var copy = (ApplicationUser)MemberwiseClone();
			copy.Package = package;
			copy.LastPackageChange = DateTime.UtcNow;
			return copy;
		}

		public ApplicationUser WithDailyUploadSize(long newSize)
		{
			var copy = (ApplicationUser)MemberwiseClone();
			copy.DailyUploadSize = newSize;
			copy.LastUploadReset = DateTime.UtcNow;
			return copy;
		}
	}
}
