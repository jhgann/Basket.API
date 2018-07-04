using Basket.Domain.Aggregates;
using System.Collections.Generic;

namespace Basket.API.Services
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
