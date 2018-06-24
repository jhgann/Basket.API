using Basket.API.IntegrationEvents.Events;
using Basket.API.Models;
using Basket.API.Services;
using EventBusCore.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.IntegrationEvents.EventHandlers
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;

        public ProductPriceChangedIntegrationEventHandler(IBasketRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(ProductPriceChangedIntegrationEvent @event)
        {
            var baskets = _repository.GetBaskets();

            foreach (var basket in baskets)
            {
                await UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, basket);
            }
        }

        private async Task UpdatePriceInBasketItems(string productId, decimal newPrice, ShoppingBasket basket)
        {
            var itemsToUpdate = basket?.BasketItems?.Where(x => x.ProductId == productId).ToList();

            if (itemsToUpdate != null)
            {
                foreach (var item in itemsToUpdate)
                {
                    if (item.ProductPrice != newPrice)
                    {
                        item.ProductPrice = newPrice;
                    }
                }
                _repository.UpdateBasket(basket);
            }
        }
    }
}
