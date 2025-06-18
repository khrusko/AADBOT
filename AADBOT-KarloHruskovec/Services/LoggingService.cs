using AADBOT_KarloHruskovec.Events;
using System.Collections.Concurrent;

namespace AADBOT_KarloHruskovec.Services
{
	public class LoggingService : IEventBus
	{
		private static readonly Lazy<LoggingService> _instance =
			new Lazy<LoggingService>(() => new LoggingService());

		public static LoggingService Instance => _instance.Value;

		private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers;
		private LoggingService()
		{
			_handlers = new ConcurrentDictionary<Type, List<Func<object, Task>>>();
		}

		public Task PublishAsync<T>(T evt)
		{
			var eventType = typeof(T);

			if (_handlers.TryGetValue(eventType, out var registeredHandlers))
			{
				var tasks = registeredHandlers.Select(h => h(evt));
				return Task.WhenAll(tasks);
			}

			return Task.CompletedTask;
		}

		public void Subscribe<T>(Func<T, Task> handler)
		{
			var eventType = typeof(T);
			var wrapper = new Func<object, Task>(e => handler((T)e));

			_handlers.AddOrUpdate(
				eventType,
				_ => new List<Func<object, Task>> { wrapper },
				(_, existing) =>
				{
					existing.Add(wrapper);
					return existing;
				});
		}
	}
}
