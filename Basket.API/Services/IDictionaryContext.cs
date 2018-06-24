using System.Collections.Generic;
using Basket.API.Models;

namespace Basket.API.Services
{
    public interface IDictionaryContext
    {
        /// <summary>
        /// The in memory 'database' to use for this prototype.
        /// </summary>
        Dictionary<string, ShoppingBasket> Baskets { get; }
    }
}