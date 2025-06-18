using AADBOT_KarloHruskovec.DTOs;
using AADBOT_KarloHruskovec.Services;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class ImageService : IImageService
{
	public async Task<(byte[] bytes, string newFileName, string format)> ProcessImageAsync(IFormFile file, string format, int? resizeToWidth)
	{
		format = format.ToLower();
		var fileExt = format switch
		{
			"jpg" => ".jpg",
			"jpeg" => ".jpg",
			"png" => ".png",
			"bmp" => ".bmp",
			_ => ".jpg"
		};

		using var stream = file.OpenReadStream();
		using var image = Image.FromStream(stream);
		int width = resizeToWidth ?? image.Width;
		int height = (int)(image.Height * (width / (double)image.Width));
		using var resized = new Bitmap(image, new Size(width, height));

		using var ms = new MemoryStream();
		var codec = format switch
		{
			"png" => ImageFormat.Png,
			"bmp" => ImageFormat.Bmp,
			_ => ImageFormat.Jpeg
		};
		resized.Save(ms, codec);
		return (ms.ToArray(), Guid.NewGuid().ToString() + fileExt, format);
	}
	public async Task<byte[]> ApplyFiltersAsync(byte[] originalBytes, DownloadFilterOptions filters)
	{
		using var ms = new MemoryStream(originalBytes);
		using var image = Image.FromStream(ms);

		int width = filters.ResizeWidth ?? image.Width;
		int height = (int)(image.Height * (width / (double)image.Width));
		using var resized = new Bitmap(image, new Size(width, height));

		if (filters.Sepia || filters.Blur)
		{
			for (int y = 0; y < resized.Height; y++)
			{
				for (int x = 0; x < resized.Width; x++)
				{
					var pixel = resized.GetPixel(x, y);

					if (filters.Sepia)
					{
						int tr = (int)(0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B);
						int tg = (int)(0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B);
						int tb = (int)(0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B);
						resized.SetPixel(x, y, Color.FromArgb(Clamp(tr), Clamp(tg), Clamp(tb)));
					}

					if (filters.Blur)
					{
						
						var neighbors = new List<Color>();
						for (int dx = -1; dx <= 1; dx++)
							for (int dy = -1; dy <= 1; dy++)
							{
								int nx = x + dx, ny = y + dy;
								if (nx >= 0 && nx < resized.Width && ny >= 0 && ny < resized.Height)
									neighbors.Add(resized.GetPixel(nx, ny));
							}
						var avg = Color.FromArgb(
							((int)neighbors.Average(c => c.R)),
							((int)neighbors.Average(c => c.G)),
							((int)neighbors.Average(c => c.B)));
						resized.SetPixel(x, y, avg);
					}
				}
			}
		}

		using var outStream = new MemoryStream();
		var codec = filters.Format switch
		{
			"png" => ImageFormat.Png,
			"bmp" => ImageFormat.Bmp,
			_ => ImageFormat.Jpeg
		};
		resized.Save(outStream, codec);
		return outStream.ToArray();
	}

	private int Clamp(int val) => Math.Max(0, Math.Min(255, val));

}
