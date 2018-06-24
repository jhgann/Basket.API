﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Models
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
        public string CustomerId { get; set; }

        /// <summary>
        /// A collection of the items contained in this basket.
        /// </summary>
        public List<BasketItem> BasketItems { get; } = new List<BasketItem>();
    }
}
