using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Models;
using AADBOT_KarloHruskovec.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
		public async Task<IActionResult> Upload(
			[FromForm] IFormFile file,
			[FromForm] string description,
			[FromForm] string hashtags,
			[FromForm] string format = "jpg",
			[FromForm] int? resize = null)
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
		public async Task<IActionResult> Edit(int id, [FromBody] PhotoUpdate updated)
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

		[HttpGet("unsafe-author-search")]
		public async Task<IActionResult> UnsafeSearch(string author)
		{
			var result = await _photoService.UnsafeSearchByAuthorRaw(author);
			return Ok(result);
		}


		[HttpPost("{id}/download")]
		public async Task<IActionResult> DownloadProcessed(int id, [FromBody] DownloadFilterOptions filters)
		{
			var bytes = await _photoService.GetFilteredPhotoAsync(id, filters);
			if (bytes == null) return NotFound();
			return File(bytes, "application/octet-stream");
		}

		[HttpPost("serialize-settings")]
		public IActionResult SerializeSettings([FromBody] ImageSettings input)
		{
			var json = JsonSerializer.Serialize(input, new JsonSerializerOptions { WriteIndented = true });
			return Ok(new { RawJson = json });
		}

		[HttpPost("deserialize-insecure")]
		public IActionResult DeserializeInsecure([FromBody] JsonElement inputJson)
		{
			if (!inputJson.TryGetProperty("json", out var rawJsonElement))
				return BadRequest(new { Error = "Missing 'json' property." });

			var rawJson = rawJsonElement.GetString();
			try
			{
				var obj = JsonSerializer.Deserialize<ImageSettings>(rawJson!);
				return Ok(new { Confirmed = $"{obj.Width}x{obj.Height} with {obj.Filter}" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { Error = ex.Message });
			}
		}


		[HttpPost("deserialize-secure")]
		public IActionResult DeserializeSecure([FromBody] JsonElement inputJson)
		{
			if (!inputJson.TryGetProperty("json", out var rawJsonElement))
				return BadRequest(new { Error = "Missing 'json' property." });

			var rawJson = rawJsonElement.GetString();

			var options = new JsonSerializerOptions
			{
				TypeInfoResolver = JsonTypeInfoResolver.Combine(
					new DefaultJsonTypeInfoResolver
					{
						Modifiers =
						{
					typeInfo =>
					{
						if (typeInfo.Type == typeof(ImageSettings))
							return;

						if (typeInfo.Type.Namespace?.StartsWith("System") == true)
							return;

						throw new NotSupportedException($"Type {typeInfo.Type} is not allowed.");
					}
						}
					}
				)
			};

			try
			{
				var obj = JsonSerializer.Deserialize<ImageSettings>(rawJson!, options);
				if (obj == null || obj.Width == 0 || obj.Height == 0 || string.IsNullOrWhiteSpace(obj.Filter))
				{
					throw new NotSupportedException("Input is not a valid ImageSettings object.");
				}

				return Ok(new { Confirmed = $"{obj.Width}x{obj.Height} with {obj.Filter}" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { Error = ex.Message });
			}
		}




	}
}
