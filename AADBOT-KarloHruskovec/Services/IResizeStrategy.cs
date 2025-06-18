using System.Drawing;

namespace AADBOT_KarloHruskovec.Services
{
	public interface IResizeStrategy
	{
		Task<Image> ResizeAsync(Image image);
	}
}
