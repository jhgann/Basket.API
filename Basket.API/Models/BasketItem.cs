using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Models
{
    public class BasketItem
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        [Required]
        [Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999.")] //TODO: Make max configurable
        public int Quantity { get; set; }
    }
}
