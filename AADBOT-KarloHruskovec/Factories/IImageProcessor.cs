namespace AADBOT_KarloHruskovec.Factories
{
	public interface IImageProcessor
	{
		Task<byte[]> ProcessAsync(IFormFile file, int? resize);
	}

}
