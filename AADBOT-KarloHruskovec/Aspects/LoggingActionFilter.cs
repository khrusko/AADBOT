using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AADBOT_KarloHruskovec.Aspects
{
	public class LoggingActionFilter : IActionFilter
	{
		private readonly ILogger<LoggingActionFilter> _logger;
		private Stopwatch _stopwatch;

		public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
		{
			_logger = logger;
			_stopwatch = new Stopwatch();
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			_stopwatch.Restart();
			_logger.LogInformation("Executing {Action} at {Time}", context.ActionDescriptor.DisplayName, DateTime.UtcNow);
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			_stopwatch.Stop();
			_logger.LogInformation("Executed {Action} in {Elapsed} ms", context.ActionDescriptor.DisplayName, _stopwatch.ElapsedMilliseconds);
		}
	}
}
