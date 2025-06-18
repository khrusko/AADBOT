namespace AADBOT_KarloHruskovec.Events
{
	public interface IEventBus
	{
		Task PublishAsync<T>(T evt);
		void Subscribe<T>(Func<T, Task> handler);
	}
}
