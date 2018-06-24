using Basket.API.Models;
using System.Collections.Generic;

namespace Basket.API.Services
{
    public interface IBasketRepository
    {
        // TODO: add comments
        ICollection<ShoppingBasket> GetBaskets();

        bool TryGetBasket(string customerId, out ShoppingBasket basket);

        bool TryGetItemInBasket(string itemId, ShoppingBasket basket, out BasketItem item);

        ShoppingBasket UpdateBasket(ShoppingBasket basket);

        void DeleteBasket(string customerId);
    }
}
