﻿namespace AADBOT_KarloHruskovec.Models
{
	public class LogEntry
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Action { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
