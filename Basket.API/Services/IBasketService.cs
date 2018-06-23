using Basket.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Services
{
    public interface IBasketService
    {
        void AddItemToBasket(ShoppingBasket basket, BasketItem item);

        void RemoveItemFromBasket(ShoppingBasket basket, BasketItem item);

        bool TryUpdateItemInBasket(ShoppingBasket basket, BasketItem item, JsonPatchDocument<BasketItem> patch, out ICollection<ValidationResult> validationResults);
    }
}
