namespace AADBOT_KarloHruskovec.Factories
{
	public class ImageProcessorFactory
	{
		public IImageProcessor GetProcessor(string format)
		{
			return format.ToLower() switch
			{
				"jpg" or "jpeg" => new JpegImageProcessor(),
				"png" => new PngImageProcessor(),
				_ => throw new NotSupportedException($"Format {format} is not supported.")
			};
		}
	}

}
