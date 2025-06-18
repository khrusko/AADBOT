namespace AADBOT_KarloHruskovec.Events
{
	public class PhotoUploadedEvent
	{
		public string FileName { get; set; }
		public string UserId { get; set; }

		public PhotoUploadedEvent(string fileName, string userId)
		{
			FileName = fileName;
			UserId = userId;
		}
	}
}
