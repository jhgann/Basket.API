using Basket.Domain.Aggregates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Services
{
    //TODO: is this service still needed, now that business logic is moved to domain object?

    /// <summary>
    /// Service to handle business logic related to shopping baskets.
    /// </summary>
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<BasketService> _logger;

        public BasketService(IBasketRepository basketRepository, ILogger<BasketService> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void AddItemToBasket(ShoppingBasket basket, BasketItem item)
        {
            basket.AddItemToBasket(item);
            _basketRepository.UpdateBasket(basket);
        }

        public void RemoveItemFromBasket(ShoppingBasket basket, BasketItem item)
        {
            basket.RemoveItemFromBasket(item.ProductId);
            _basketRepository.UpdateBasket(basket);
        }

        public bool TryUpdateBasketItemQuantity(ShoppingBasket basket, BasketItem item, int newQuantity, out ICollection<ValidationResult> validationResults)
        {
            var isValid = basket.TryUpdateBasketItemQuantity(item.ProductId, newQuantity, out validationResults);

            if (isValid)
            {
                _basketRepository.UpdateBasket(basket);
            }
            return isValid;
        }
    }
}
