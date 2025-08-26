namespace AADBOT_KarloHruskovec.DTOs
{
	public class DownloadFilterOptions
	{
		public int? ResizeWidth { get; set; }
		public bool? Sepia { get; set; }
		public bool? Blur { get; set; }
		public string Format { get; set; } = "jpg";
	}

}
