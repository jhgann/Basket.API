using System.ComponentModel.DataAnnotations;

namespace Basket.Domain.Aggregates
{
    /// <summary>
    /// A single item in a shopping basket.
    /// </summary>
    public class BasketItem
    {
        /// <summary>
        /// Id of the product.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ProductId { get; private set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProductName { get; private set; }

        /// <summary>
        /// Price of the product.
        /// </summary>
        [Required]
        public decimal ProductPrice { get; private set; }

        /// <summary>
        /// Quantity of the product.
        /// </summary>
        [Required]
        [Range(1, 999, ErrorMessage = "Quantity must be between {1} and {2}.")]
        public int Quantity { get; private set; }

        public BasketItem(string productId, string productName, decimal productPrice, int quantity)
        {
            //TODO: perform validations in ctor instead of attributes?

            ProductId = productId;
            ProductName = productName;
            ProductPrice = productPrice;
            Quantity = quantity;
        }

        /// <summary>
        /// Updates the price of the product.  
        /// This is only accessible through the aggregate root (shopping basket domain model).
        /// </summary>
        internal void UpdatePrice(decimal newPrice)
        {
            ProductPrice = newPrice;
        }

        /// <summary>
        /// Updates the quantity of the product.  
        /// This is only accessible through the aggregate root (shopping basket domain model).
        /// </summary>
        internal void UpdateQuantity(int newQuantity) {
            Quantity = newQuantity;
        }
    }
}
