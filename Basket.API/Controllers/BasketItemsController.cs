using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Basket.API.Models;
using Basket.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/ShoppingBaskets/{customerId}/[controller]")]
    [ApiController]
    public class BasketItemsController : ControllerBase
    {
        private readonly IBasketRepository _cacheRepository;
        private readonly IBasketService _basketService;
        private readonly ILogger<BasketItemsController> _logger;

        public BasketItemsController(IBasketRepository cacheRepository, IBasketService basketService, ILogger<BasketItemsController> logger)
        {
            _cacheRepository = cacheRepository;
            _basketService = basketService;
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

            _basketService.AddItemToBasket(shoppingBasket, item);

            return CreatedAtRoute("GetItemInBasket", new { customerId, itemId = item.ProductId }, item);
        }

        [HttpPatch("{itemId}")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> UpdateQuantity(string customerId, string itemId, [FromBody]JsonPatchDocument<BasketItem>  patch)
        {
            // Try to get a basket to add the item to
            var foundBasket = _cacheRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket);
            if (!foundBasket)
            {
                return BasketNotFound(customerId);
            }

            // Check if item already exists in basket
            var foundItem = _cacheRepository.TryGetItemInBasket(itemId, shoppingBasket, out BasketItem item);
            if (!foundItem)
            {
                return ItemNotFound(customerId, itemId);
            }

            // Validate change and update via service
            var isValid = _basketService.TryUpdateItemInBasket(shoppingBasket, item, patch, out ICollection<ValidationResult> validationResults);
            if (!isValid)
            {
                return BadRequest(validationResults);
            }

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

            // Check if item already exists in basket
            var foundItem = _cacheRepository.TryGetItemInBasket(itemId, shoppingBasket, out BasketItem item);
            if (!foundItem)
            {
                return ItemNotFound(customerId, itemId);
            }

            _basketService.RemoveItemFromBasket(shoppingBasket, item);

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
