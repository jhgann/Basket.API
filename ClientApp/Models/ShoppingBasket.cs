using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    /// <summary>
    /// A model for the shopping basket
    /// Each customer has a single basket identified by the customer's id.
    /// </summary>
    public class ShoppingBasket
    {
        [Required]
        [StringLength(50)]
        public string CustomerId { get; set; }

        public List<BasketItem> BasketItems { get; } = new List<BasketItem>();
    }
}
