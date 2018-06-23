using Basket.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Services
{
    /// <summary>
    /// Service to handle business logic related to shopping baskets.
    /// </summary>
    public class BasketService : IBasketService
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly ILogger<BasketService> _logger;

        public BasketService(ICacheRepository cacheRepository, ILogger<BasketService> logger)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public void AddItemToBasket(ShoppingBasket basket, BasketItem item)
        {
            basket.BasketItems.Add(item);
            _cacheRepository.UpdateBasket(basket);
        }

        public void RemoveItemFromBasket(ShoppingBasket basket, BasketItem item)
        {
            basket.BasketItems.Remove(item);
            _cacheRepository.UpdateBasket(basket);
        }

        public bool TryUpdateItemInBasket(ShoppingBasket basket, BasketItem item, JsonPatchDocument<BasketItem> patch, out ICollection<ValidationResult> validationResults)
        {
            patch.ApplyTo(item);

            var context = new ValidationContext(item);
            validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(item, context, validationResults, true);

            if (isValid)
            {
                _cacheRepository.UpdateBasket(basket);
            }
            return isValid;
        }
    }
}
