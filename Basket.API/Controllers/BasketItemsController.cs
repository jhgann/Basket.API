using System.Collections.Generic;
using System.Net;
using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/ShoppingBaskets/{customerId}/[controller]")]
    [ApiController]
    public class BasketItemsController : ControllerBase
    {
        ICacheRepository _cacheRepository;
        ILogger<BasketItemsController> _logger;

        public BasketItemsController(ICacheRepository cacheRepository, ILogger<BasketItemsController> logger)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BasketItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<List<BasketItem>> GetItemsInBasket(string customerId)
        {
            var found = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket);
            if (!found)
            {
                return BasketNotFound(customerId);
            }
            return shoppingBasket.BasketItems;
        }

        [HttpGet("{itemId}", Name = "GetItemInBasket" )]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> GetItemInBasket(string customerId, string itemId)
        {
            var foundBasket = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket);
            if (!foundBasket)
            {
                return BasketNotFound(customerId);
            }

            var foundItem = _cacheRepository.TryGetItemInBasket(itemId, shoppingBasket, out BasketItem item);
            if (!foundItem)
            {
                return ItemNotFound(customerId, itemId);
            }

            return item;
        }

        [HttpPost]
        [Route("AddItemToBasket")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> AddItemToBasket(string customerId, [FromBody] BasketItem item)
        {
            // Try to get a basket to add the item to
            var foundBasket = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket);
            if (!foundBasket)
            {
                return BasketNotFound(customerId);
            }

            // Check if item already exists in basket
            var foundItem = _cacheRepository.TryGetItemInBasket(item.ProductId, shoppingBasket, out BasketItem existingItem);
            if (foundItem)
            {
                return ItemAlreadyFound(customerId, item.ProductId);
            }

            //TODO: move logic out
            // Add the item to the basket
            shoppingBasket.BasketItems.Add(item);
            _cacheRepository.UpdateBasket(shoppingBasket);

            return CreatedAtRoute("GetItemInBasket", new { customerId, itemId = item.ProductId }, item);
        }

        //TODO: better way to represent the quantity than fromBody?
        [HttpPut]
        [Route("{itemId}/UpdateQuantity")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> UpdateQuantity(string customerId, string itemId, [FromBody]int quantity)
        {
            // Try to get a basket to add the item to
            var foundBasket = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket);
            if (!foundBasket)
            {
                return BasketNotFound(customerId);
            }

            var foundItem = _cacheRepository.TryGetItemInBasket(itemId, shoppingBasket, out BasketItem item);
            if (!foundItem)
            {
                return ItemNotFound(customerId, itemId);
            }

            //TODO: move to service?
            item.Quantity = quantity;
            TryValidateModel(item);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _cacheRepository.UpdateBasket(shoppingBasket);
            return item;
        }


        [HttpDelete("{itemId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult RemoveItemFromBasket(string customerId, string itemId)
        {
            // Try to get a basket to remove the item from
            var foundBasket = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket);
            if (!foundBasket)
            {
                return BasketNotFound(customerId);
            }

            var foundItem = _cacheRepository.TryGetItemInBasket(itemId, shoppingBasket, out BasketItem item);
            if (!foundItem)
            {
                return ItemNotFound(customerId, itemId);
            }

            //TODO: move logic out
            // Add the item to the basket
            shoppingBasket.BasketItems.Remove(item);
            _cacheRepository.UpdateBasket(shoppingBasket);

            return NoContent();
        }

        

        private ActionResult ItemNotFound(string customerId, string itemId)
        {
            var message = $"Product {itemId} could not be found in basket for customer {customerId}.";
            _logger.LogWarning(message);
            return NotFound(message);
        }

        private ActionResult<BasketItem> ItemAlreadyFound(string customerId, object itemId)
        {
            var message = $"Product {itemId} already in basket for customer {customerId}, and cannot be added again.";
            _logger.LogWarning(message);
            return BadRequest(message);
        }

        private ActionResult BasketNotFound(string customerId)
        {
            var message = $"Basket for customer {customerId} not found.";
            _logger.LogWarning(message);
            return NotFound(message);
        }
    }
}
