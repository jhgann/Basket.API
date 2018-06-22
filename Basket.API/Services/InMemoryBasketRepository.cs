using Basket.API.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Basket.API.Services
{
    public class InMemoryBasketRepository : ICacheRepository
    {
        IMemoryCache _memoryCache;
        ILogger<InMemoryBasketRepository> _logger;
        public InMemoryBasketRepository(IMemoryCache memoryCache, ILogger<InMemoryBasketRepository> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public bool TryGetBasket(string customerId, out ShoppingBasket basket)
        {
            var foundBasket = _memoryCache.TryGetValue(customerId, out basket);

            if (!foundBasket)
            {
                _logger.LogWarning("Could not find basket for customer: {0}", customerId);
            }
            else
            {
                _logger.LogInformation("Found basket for customer: {0}", customerId);
            }
            return foundBasket;
        }

        public bool TryGetItemInBasket(string itemId, ShoppingBasket basket, out BasketItem foundItem)
        {
            foundItem = basket.BasketItems.Find(basketItem => basketItem.ProductId == itemId);

            if (foundItem == null)
            {
                _logger.LogWarning($"Could not find item {itemId} in basket for customer: {basket.CustomerId}");
                return false;
            }

            _logger.LogInformation($"Found product {itemId} in basket for customer: {basket.CustomerId}");
            return true;
        }

        public ShoppingBasket UpdateBasket(ShoppingBasket basket)
        {
            var result = _memoryCache.Set(basket.CustomerId, basket);
            if (result != null)
            {
                _logger.LogInformation("Updated basket for customer: {0}", basket.CustomerId);
            }
            else
            {
                // TODO: Would this ever be hit?
                _logger.LogWarning("Basket not updated for customer: {0}", basket.CustomerId);
            }
            return result;
        }

        public void DeleteBasket(string customerId)
        {
            _memoryCache.Remove(customerId);
        }


    }
}
