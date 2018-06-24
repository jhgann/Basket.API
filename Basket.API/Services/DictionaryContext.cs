using Basket.API.Models;
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
