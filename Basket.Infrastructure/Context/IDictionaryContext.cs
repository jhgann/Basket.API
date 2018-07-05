using System.Collections.Generic;
using Basket.Domain.Aggregates;

namespace Basket.Infrastructure.Context
{
    public interface IDictionaryContext
    {
        /// <summary>
        /// The in memory 'database' to use for this prototype.
        /// </summary>
        Dictionary<string, ShoppingBasket> Baskets { get; }
    }
}