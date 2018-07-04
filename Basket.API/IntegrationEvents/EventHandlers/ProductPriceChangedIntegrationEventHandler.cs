using Basket.API.IntegrationEvents.Events;
using Basket.Domain.Aggregates;
using EventBusCore.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.IntegrationEvents.EventHandlers
{
    /// <summary>
    /// When the price of a product is changed in another service, 
    /// an event will be created, currently a result of a subscription to a message broker.
    /// This event handler will be notified of the new price of a product and update any
    /// matching products across all shopping carts.
    /// </summary>
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;

        public ProductPriceChangedIntegrationEventHandler(IBasketRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Handles a product price change.
        /// </summary>
        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            var customerIds =  _repository.GetBaskets().Select(x => x.CustomerId).ToList();
                        
            foreach (var customerId in customerIds)
            {
                // Get each basket individually, becuase we are editing within this loop.
                // Potential bottleneck, consider refactor to remove need for loop.
                var found = _repository.TryGetBasket(customerId, out ShoppingBasket basket);
                if (found) { 
                    await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, basket);
                }
            }
        }

        /// <summary>
        /// Updates the price of a product in a basket in the repository.
        /// </summary>
        private async Task UpdatePriceInBasketItems(string productId, decimal newPrice, ShoppingBasket basket)
        {
            var itemsToUpdate = basket?.BasketItems?.Where(x => x.ProductId == productId).ToList();

            if (itemsToUpdate != null)
            {
                foreach (var item in itemsToUpdate)
                {
                    if (item.ProductPrice != newPrice)
                    {
                        basket.UpdatePriceOfProductInBasket(productId, newPrice);
                    }
                }
                _repository.UpdateBasket(basket);
            }
        }
    }
}
