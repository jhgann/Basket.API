﻿using System.ComponentModel.DataAnnotations;

namespace Basket.API.Models
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
        public string ProductId { get; set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        /// <summary>
        /// Price of the product.
        /// </summary>
        [Required]
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// Quantity of the product.
        /// </summary>
        [Required]
        [Range(1, 999, ErrorMessage = "Quantity must be between {1} and {2}.")]
        public int Quantity { get; set; }
    }
}
