using System.Drawing;

namespace AADBOT_KarloHruskovec.Services
{
	public class ResizeStrategy : IResizeStrategy
	{
		private readonly int _targetWidth;

		public ResizeStrategy(int targetWidth)
		{
			_targetWidth = targetWidth;
		}

		public Task<Image> ResizeAsync(Image image)
		{
			int newHeight = (int)(image.Height * (_targetWidth / (double)image.Width));
			var resized = new Bitmap(image, new Size(_targetWidth, newHeight));
			return Task.FromResult((Image)resized);
		}
	}
}
