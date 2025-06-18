using AADBOT_KarloHruskovec.Data;
using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AADBOT_KarloHruskovec.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PackageController : ControllerBase
	{
		private readonly IPackageService _packageService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public PackageController(IPackageService packageService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
		{
			_packageService = packageService;
			_userManager = userManager;
			_context = context;
		}

		[Authorize]
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


		[Authorize]
		[HttpPost("change")]
		public async Task<IActionResult> ChangePackage([FromBody] PackageChangeRequest request)
		{
			if (!new[] { "FREE", "PRO", "GOLD" }.Contains(request.NewPackage))
				return BadRequest("Invalid package.");

			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();

			var today = DateTime.UtcNow.Date;
			var lastChange = user.LastPackageChange.Value.Date;

			if (lastChange == today)
				return BadRequest("You can only change your package once per day.");

			// schedule change for tomorrow
			user.LastPackageChange = DateTime.UtcNow;
			user.Package = request.NewPackage;

			_context.Logs.Add(new LogEntry
			{
				UserId = user.Id,
				Action = $"Scheduled package change to {request.NewPackage} (effective tomorrow)",
				Timestamp = DateTime.UtcNow
			});

			await _userManager.UpdateAsync(user);
			await _context.SaveChangesAsync();

			return Ok(new { Message = "Package change will be applied tomorrow." });
		}

	}
}
