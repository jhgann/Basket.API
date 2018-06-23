using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Models
{
    public class ShoppingBasket
    {
        [Required]
        public string CustomerId { get; set; }

        public List<BasketItem> BasketItems { get; } = new List<BasketItem>();
    }
}
