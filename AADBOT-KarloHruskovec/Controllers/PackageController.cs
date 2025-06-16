using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AADBOT_KarloHruskovec.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PackageController : ControllerBase
	{
		private readonly IPackageService _packageService;
		private readonly UserManager<ApplicationUser> _userManager;

		public PackageController(IPackageService packageService, UserManager<ApplicationUser> userManager)
		{
			_packageService = packageService;
			_userManager = userManager;
		}

		[HttpGet("status")]
		public async Task<IActionResult> GetStatus()
		{
			var user = await _userManager.GetUserAsync(User);
			var result = await _packageService.GetUserStatusAsync(user.Id);

			return Ok(new
			{
				Package = result.package,
				DailyUsed = result.used,
				DailyLimit = result.limit,
				CanChangePackage = result.canChange
			});
		}

		[HttpPost("change")]
		public async Task<IActionResult> ChangePackage([FromBody] PackageChangeRequest request)
		{
			var user = await _userManager.GetUserAsync(User);
			var result = await _packageService.ChangePackageAsync(user.Id, request.NewPackage);

			if (!result.success) return BadRequest(result.message);
			return Ok(new { Message = result.message });
		}
	}
}
