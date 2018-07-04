using System;
using System.Collections.Generic;
using System.Linq;
using Basket.Domain.Aggregates;
using Microsoft.Extensions.Logging;

namespace Basket.API.Services
{
    public class InMemoryBasketRepository : IBasketRepository
    {
        private readonly IDictionaryContext _dictionaryContext;
        private readonly ILogger<InMemoryBasketRepository> _logger;

        public InMemoryBasketRepository(IDictionaryContext dictionaryContext, ILogger<InMemoryBasketRepository> logger)
        {
            _dictionaryContext = dictionaryContext ?? throw new ArgumentNullException(nameof(dictionaryContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ICollection<ShoppingBasket> GetBaskets()
        {
            return _dictionaryContext.Baskets.Values;
        }

        public bool TryGetBasket(string customerId, out ShoppingBasket basket)
        {
            var foundBasket = _dictionaryContext.Baskets.TryGetValue(customerId, out basket);

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

        public bool TryGetItemInBasket(string itemId, string customerId, out BasketItem foundItem)
        {
            foundItem = _dictionaryContext.Baskets[customerId].BasketItems.FirstOrDefault(basketItem => basketItem.ProductId == itemId);

            if (foundItem == null)
            {
                _logger.LogWarning($"Could not find item {itemId} in basket for customer: {customerId}");
                return false;
            }

            _logger.LogInformation($"Found product {itemId} in basket for customer: {customerId}");
            return true;
        }

        public ShoppingBasket UpdateBasket(ShoppingBasket basket)
        {
            var result = _dictionaryContext.Baskets[basket.CustomerId] = basket;
            if (result != null)
            {
                _logger.LogInformation("Updated basket for customer: {0}", basket.CustomerId);
            }
            else
            {
                _logger.LogWarning("Basket not updated for customer: {0}", basket.CustomerId);
            }
            return result;
        }

        public void DeleteBasket(string customerId)
        {
            _dictionaryContext.Baskets.Remove(customerId);
        }
    }
}
