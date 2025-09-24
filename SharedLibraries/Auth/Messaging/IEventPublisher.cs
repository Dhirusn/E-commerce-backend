using System.Threading.Tasks;

namespace Shared.Library.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, string topicOrRoutingKey = "") where TEvent : class;
    }
}
