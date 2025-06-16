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
	public class PhotoController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IPhotoService _photoService;

		public PhotoController(UserManager<ApplicationUser> userManager, IPhotoService photoService)
		{
			_userManager = userManager;
			_photoService = photoService;
		}

		[HttpPost("upload")]
		[Authorize]
		public async Task<IActionResult> Upload(IFormFile file, string description, string hashtags, string format = "jpg", int? resize = null)
		{
			var user = await _userManager.GetUserAsync(User);
			var result = await _photoService.UploadPhotoAsync(user, file, description, hashtags, format, resize);
			return result ? Ok(new { Message = "Uploaded successfully." }) : BadRequest();
		}

		[HttpGet("latest")]
		public async Task<IActionResult> GetLatest()
		{
			var photos = await _photoService.GetLatestPhotosAsync();
			return Ok(photos);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetPhoto(int id)
		{
			var bytes = await _photoService.GetPhotoFileAsync(id);
			if (bytes == null) return NotFound();
			return File(bytes, "application/octet-stream");
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Edit(int id, [FromBody] Photo updated)
		{
			var user = await _userManager.GetUserAsync(User);
			var success = await _photoService.UpdatePhotoAsync(user, id, updated.Description, updated.Hashtags);
			return success ? Ok(updated) : Forbid();
		}

		[HttpGet("search")]
		public async Task<IActionResult> Search(string? hashtag, string? author, long? minSize, long? maxSize, DateTime? from, DateTime? to)
		{
			var result = await _photoService.SearchPhotosAsync(hashtag, author, minSize, maxSize, from, to);
			return Ok(result);
		}

		[HttpPost("{id}/download")]
		public async Task<IActionResult> DownloadProcessed(int id, [FromBody] DownloadFilterOptions filters)
		{
			var bytes = await _photoService.GetFilteredPhotoAsync(id, filters);
			if (bytes == null) return NotFound();
			return File(bytes, "application/octet-stream");
		}

	}
}
