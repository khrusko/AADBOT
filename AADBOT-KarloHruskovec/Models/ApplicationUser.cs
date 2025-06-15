using Microsoft.AspNetCore.Identity;

namespace AADBOT_KarloHruskovec.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Package { get; set; } = "FREE"; // FREE, PRO, GOLD
		public DateTime? LastPackageChange { get; set; }
		public long DailyUploadSize { get; set; } = 0; // in bytes
		public DateTime? LastUploadReset { get; set; }
	}
}
