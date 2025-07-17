namespace SharedLibraries.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T eventData, string? routingKey = null) where T : class;
        Task SubscribeAsync<T>(Func<T, Task> handler, string? routingKey = null) where T : class;
        void Subscribe<T>(Func<T, Task> handler, string? routingKey = null) where T : class;
        void Unsubscribe<T>() where T : class;
    }
    
    public interface IEventHandler<in T> where T : class
    {
        Task HandleAsync(T eventData);
    }
}
