using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Basket.API.Controllers
{
    /// <summary>
    /// Because the cache repo doesn't use async methods, actions here are not async.
    /// If using a network data store, these would of course be async.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ShoppingBasketsController : ControllerBase
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly ILogger<ShoppingBasketsController> _logger;
        public ShoppingBasketsController(ICacheRepository cacheRepository, ILogger<ShoppingBasketsController> logger)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        [HttpGet("{customerId}", Name = "Get")]
        [ProducesResponseType(typeof(ShoppingBasket), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<ShoppingBasket> Get(string customerId)
        {
            var found = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket basket);
            if (!found)
            {
                return BasketNotFound(customerId);
            }
            return basket;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingBasket), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<ShoppingBasket> Post([FromBody] ShoppingBasket basket)
        {
            var basketResult = _cacheRepository.UpdateBasket(basket);
            return CreatedAtRoute("Get", new { customerId = basketResult.CustomerId }, basketResult);
        }

        [HttpDelete("{customerId}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public ActionResult Delete(string customerId)
        {
            var found = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket basket);
            if (!found)
            {
                return BasketNotFound(customerId);
            }

            _cacheRepository.DeleteBasket(customerId);
            return NoContent();
        }

        private ActionResult BasketNotFound(string customerId)
        {
            var message = $"Basket for customer {customerId} not found.";
            _logger.LogWarning(message);
            return NotFound(message);
        }
    }
}
