using System.Collections.Generic;

namespace Basket.Domain.Aggregates
{
    /// <summary>
    /// Methods to implement to interact with the shopping basket repository.
    /// </summary>
    public interface IBasketRepository
    {
        /// <summary>
        /// Gets all of the shopping baskets
        /// </summary>
        ICollection<ShoppingBasket> GetBaskets();

        /// <summary>
        /// Tries to get a basket for a customer.
        /// </summary>
        /// <returns>True and the basket if one exists for the customer. False otherwise.</returns>
        bool TryGetBasket(string customerId, out ShoppingBasket basket);

        /// <summary>
        /// Tries to get an item within a customer's basket
        /// </summary>
        /// <returns>True and the item if one exists in the customer's basket. False otherwise.</returns>
        bool TryGetItemInBasket(string itemId, string customerId, out BasketItem item);

        /// <summary>
        /// Creates or updates a shopping basket in the repository.
        /// </summary>
        /// <returns>The resulting basket from the repository.</returns>
        ShoppingBasket UpdateBasket(ShoppingBasket basket);

        /// <summary>
        /// Deletes the customer's shopping basket from the repository, if it exists.
        /// </summary>
        void DeleteBasket(string customerId);
    }
}
