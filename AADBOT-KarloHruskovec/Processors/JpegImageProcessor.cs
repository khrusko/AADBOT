using AADBOT_KarloHruskovec.Factories;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class JpegImageProcessor : IImageProcessor
{
	public async Task<byte[]> ProcessAsync(IFormFile file, int? resize)
	{
		using var stream = file.OpenReadStream();
		using var image = Image.FromStream(stream);

		Image resized = image;
		if (resize.HasValue && resize.Value > 0)
		{
			int width = resize.Value;
			int height = (int)(image.Height * (resize.Value / (double)image.Width));
			resized = new Bitmap(image, new Size(width, height));
		}

		using var ms = new MemoryStream();
		resized.Save(ms, ImageFormat.Jpeg);
		return ms.ToArray();
	}
}
