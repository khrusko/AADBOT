using System.Collections.Generic;
using System.Linq;
using AADBOT_KarloHruskovec.Models;

namespace AADBOT_KarloHruskovec.Application.Common;

public static class UserTransforms
{
	public static IReadOnlyList<ApplicationUser> NormalizeEmails(IEnumerable<ApplicationUser> users) =>
		users.Select(u => u.WithEmail(u.Email.Trim().ToLowerInvariant())).ToList();

	public static IReadOnlyList<ApplicationUser> UpgradePackages(IEnumerable<ApplicationUser> users, string oldPkg, string newPkg) =>
		users.Select(u => u.Package == oldPkg ? u.WithPackage(newPkg) : u).ToList();

	public static IReadOnlyList<ApplicationUser> ResetUploadSizes(IEnumerable<ApplicationUser> users) =>
		users.Select(u => u.WithDailyUploadSize(0)).ToList();
}
