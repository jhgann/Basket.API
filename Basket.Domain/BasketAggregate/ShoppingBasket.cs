using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Basket.Domain.Aggregates
{
    /// <summary>
    /// A model for the shopping basket.
    /// Each customer has a single basket identified by the customer's id.
    /// </summary>
    public class ShoppingBasket
    {
        /// <summary>
        /// The id of the customer this basket is for.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CustomerId { get; private set; }

        /// <summary>
        /// A collection of the items contained in this basket.
        /// </summary>
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems;
        private readonly List<BasketItem> _basketItems;

        public ShoppingBasket(string customerId)
        {
            CustomerId = customerId;
            _basketItems = new List<BasketItem>();
        }

        public bool IsItemInBasket(string productId)
        {
            return _basketItems.Any(basketItem => basketItem.ProductId == productId);
        }

        public void AddItemToBasket(BasketItem basketItem) {
            if (!IsItemInBasket(basketItem.ProductId))
            {
                _basketItems.Add(basketItem);
            }
        }

        public void RemoveItemFromBasket(string productId) {
            if (IsItemInBasket(productId))
            {
                var basketItem = _basketItems.Single(item => item.ProductId == productId);
                _basketItems.Remove(basketItem);
            }
        }

        public bool TryUpdateBasketItemQuantity(string productId, int newQuantity, out ICollection<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            if (IsItemInBasket(productId))
            {
                var basketItem = _basketItems.Single(item => item.ProductId == productId);
                basketItem.UpdateQuantity(newQuantity);

                var context = new ValidationContext(basketItem);
                var isValid = Validator.TryValidateObject(basketItem, context, validationResults, true);

                return isValid;
            }
            validationResults.Add(new ValidationResult("Product not found in basket."));
            return false;
        }

        public void UpdatePriceOfProductInBasket(string productId, decimal newPrice)
        {
            if (IsItemInBasket(productId))
            {
                var basketItem = _basketItems.Single(item => item.ProductId == productId);
                basketItem.UpdatePrice(newPrice); 
            }
        }
    }
}
