using AADBOT_KarloHruskovec.Services;
using AADBOT_KarloHruskovec.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AADBOT_KarloHruskovec.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _adminService;

		public AdminController(IAdminService adminService)
		{
			_adminService = adminService;
		}

		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _adminService.GetAllUsersAsync();
			return Ok(users);
		}

		[HttpPut("user/{id}/package")]
		public async Task<IActionResult> UpdateUserPackage(string id, [FromBody] string newPackage)
		{
			var success = await _adminService.UpdateUserPackageAsync(id, newPackage);
			if (!success) return NotFound();
			return Ok(new { Message = "User package updated." });
		}

		[HttpGet("photos")]
		public async Task<IActionResult> GetAllPhotos()
		{
			var photos = await _adminService.GetAllPhotosAsync();
			return Ok(photos);
		}

		[HttpDelete("photo/{id}")]
		public async Task<IActionResult> DeletePhoto(int id)
		{
			var userId = User.Identity?.Name ?? "UnknownAdmin";
			var success = await _adminService.DeletePhotoAsync(id, userId);
			if (!success) return NotFound();
			return Ok(new { Message = "Photo deleted." });
		}

		[HttpGet("logs")]
		public async Task<IActionResult> ViewLogs()
		{
			var logs = await _adminService.GetRecentLogsAsync();
			return Ok(logs);
		}
	}
}
