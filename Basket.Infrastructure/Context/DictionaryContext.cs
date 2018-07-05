using Basket.Domain.Aggregates;
using System.Collections.Generic;

namespace Basket.Infrastructure.Context
{
    public class DictionaryContext : IDictionaryContext
    {
        public DictionaryContext()
        {
            Baskets = new Dictionary<string, ShoppingBasket>();
        }

        public Dictionary<string, ShoppingBasket> Baskets { get; }
    }
}
