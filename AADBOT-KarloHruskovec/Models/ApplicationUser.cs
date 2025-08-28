using System;
using Microsoft.AspNetCore.Identity;

namespace AADBOT_KarloHruskovec.Models
{
	public partial class ApplicationUser : IdentityUser
	{
		public string Package { get; set; } = "FREE";
		public DateTime? LastPackageChange { get; set; }
		public long DailyUploadSize { get; set; } = 0;
		public DateTime? LastUploadReset { get; set; }
	}
}
