using EventBusCore.Events;

namespace Basket.API.IntegrationEvents.Events
{
    /// <summary>
    /// An event that is fired when the price of a product changes.
    /// Contains the product id and new price of the product.
    /// </summary>
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// Id of the product to update.
        /// </summary>
        public string ProductId { get; private set; }

        /// <summary>
        /// New price of the product.
        /// </summary>
        public decimal NewPrice { get; private set; }

        public ProductPriceChangedIntegrationEvent(string productId, decimal newPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
        }
    }
}
