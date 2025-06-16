using AADBOT_KarloHruskovec.DTOs;

public interface IImageService
{
	Task<(byte[] bytes, string newFileName, string format)> ProcessImageAsync(IFormFile file, string format, int? resizeToWidth);
	Task<byte[]> ApplyFiltersAsync(byte[] originalBytes, DownloadFilterOptions filters);

}
