using Basket.API.Models;

namespace Basket.API.Services
{
    public interface ICacheRepository
    {
        // TODO: add comments
        bool TryGetBasket(string customerId, out ShoppingBasket basket);

        bool TryGetItemInBasket(string itemId, ShoppingBasket basket, out BasketItem item);

        ShoppingBasket UpdateBasket(ShoppingBasket basket);

        void DeleteBasket(string customerId);
    }
}
