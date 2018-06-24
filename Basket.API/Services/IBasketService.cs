using Basket.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Services
{
    /// <summary>
    /// Interface for service that will handle business logic related to the shopping basket.
    /// </summary>
    public interface IBasketService
    {
        /// <summary>
        /// Adds an item to the shopping basket.
        /// </summary>
        void AddItemToBasket(ShoppingBasket basket, BasketItem item);

        /// <summary>
        /// Removes an item from the shopping basket.
        /// </summary>
        void RemoveItemFromBasket(ShoppingBasket basket, BasketItem item);

        /// <summary>
        /// Trys to update an item in the shopping basket via JsonPatch.
        /// </summary>
        /// <returns> True if update was successful. If false, validation results should contain reason why.</returns>
        bool TryUpdateItemInBasket(ShoppingBasket basket, BasketItem item, JsonPatchDocument<BasketItem> patch, out ICollection<ValidationResult> validationResults);
    }
}
