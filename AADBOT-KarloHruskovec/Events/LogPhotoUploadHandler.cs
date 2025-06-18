namespace AADBOT_KarloHruskovec.Events
{
	public class LogPhotoUploadHandler
	{
		private readonly ILogger<LogPhotoUploadHandler> _logger;

		public LogPhotoUploadHandler(ILogger<LogPhotoUploadHandler> logger)
		{
			_logger = logger;
		}

		public Task Handle(PhotoUploadedEvent evt)
		{
			_logger.LogInformation($"[Observer] Photo uploaded: {evt.FileName} by user {evt.UserId}");
			return Task.CompletedTask;
		}
	}
}
