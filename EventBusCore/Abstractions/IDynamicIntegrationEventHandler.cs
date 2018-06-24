using System.Threading.Tasks;

namespace EventBusCore.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
