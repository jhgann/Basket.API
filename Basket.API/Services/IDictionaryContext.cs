using System.Collections.Generic;
using Basket.API.Models;

namespace Basket.API.Services
{
    public interface IDictionaryContext
    {
        Dictionary<string, ShoppingBasket> Baskets { get; }
    }
}