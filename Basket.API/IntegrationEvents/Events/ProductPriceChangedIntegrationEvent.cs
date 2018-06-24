using EventBusCore.Events;

namespace Basket.API.IntegrationEvents.Events
{
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        public string ProductId { get; private set; }
        public decimal NewPrice { get; private set; }

        public ProductPriceChangedIntegrationEvent(string productId, decimal newPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
        }
    }
}
