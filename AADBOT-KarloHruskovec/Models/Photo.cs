namespace AADBOT_KarloHruskovec.Models
{
	public class Photo
	{
		public int Id { get; set; }
		public string FileName { get; set; }
		public string Format { get; set; }
		public long Size { get; set; }
		public string Description { get; set; }
		public string Hashtags { get; set; }
		public DateTime UploadDate { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
	}
}
