using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Basket.API.Controllers
{
    /// <summary>
    /// Controller used to access shopping baskets.
    /// Because the cache repo doesn't use async methods, actions here are not async.
    /// If using a network data store, these should change to be async.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShoppingBasketsController : ControllerBase
    {
        private readonly IBasketRepository _cacheRepository;
        private readonly ILogger<ShoppingBasketsController> _logger;
        public ShoppingBasketsController(IBasketRepository cacheRepository, ILogger<ShoppingBasketsController> logger)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get a shopping basket for the entered customer.
        /// </summary>
        [HttpGet("{customerId}", Name = "Get")]
        [ProducesResponseType(typeof(ShoppingBasket), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<ShoppingBasket> Get(string customerId)
        {
            if (!_cacheRepository.TryGetBasket(customerId, out ShoppingBasket basket))
            {
                return BasketNotFound(customerId);
            }
            return basket;
        }

        /// <summary>
        /// Create a new shopping basket for the customer.
        /// This will overwrite any existing basket for the customer.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingBasket), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<ShoppingBasket> Post([FromBody] ShoppingBasket basket)
        {
            var basketResult = _cacheRepository.UpdateBasket(basket);
            return CreatedAtRoute("Get", new { customerId = basketResult.CustomerId }, basketResult);
        }

        /// <summary>
        /// Deletes the shopping basket of the customer.
        /// </summary>
        [HttpDelete("{customerId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult Delete(string customerId)
        {
            if (!_cacheRepository.TryGetBasket(customerId, out ShoppingBasket basket))
            {
                return BasketNotFound(customerId);
            }

            _cacheRepository.DeleteBasket(customerId);
            return NoContent();
        }

        #region Private methods
        private ActionResult BasketNotFound(string customerId)
        {
            var message = $"Basket for customer {customerId} not found.";
            _logger.LogWarning(message);
            return NotFound(message);
        }
        #endregion  
    }
}
