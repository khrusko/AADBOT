using Prometheus;

namespace AADBOT_KarloHruskovec.Metrics
{
	public static class AuthMetrics
	{
		public static readonly Counter SuccessfulLogins =
			Prometheus.Metrics.CreateCounter("app_successful_logins_total", "Number of successful logins");

		public static readonly Counter FailedLogins =
			Prometheus.Metrics.CreateCounter("app_failed_logins_total", "Number of failed logins");

		public static readonly Counter DailyUploadResets =
			Prometheus.Metrics.CreateCounter("app_daily_upload_resets_total", "Number of daily upload quota resets");
	}
}
