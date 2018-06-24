using System;
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
    /// <summary>
    /// Controller used to access items within a shopping basket.
    /// Because the cache repo doesn't use async methods, actions here are not async.
    /// If using a network data store, these should change to be async.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/ShoppingBaskets/{customerId}/[controller]")]
    [ApiController]
    public class BasketItemsController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketService _basketService;
        private readonly ILogger<BasketItemsController> _logger;

        public BasketItemsController(IBasketRepository basketRepository, IBasketService basketService, ILogger<BasketItemsController> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all items in a shopping basket.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BasketItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<List<BasketItem>> GetItemsInBasket(string customerId)
        {
            if (!_basketRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket))
            {
                return BasketNotFound(customerId);
            }
            return shoppingBasket.BasketItems;
        }

        /// <summary>
        /// Gets a single item within a shopping basket.
        /// </summary>
        [HttpGet("{itemId}", Name = "GetItemInBasket" )]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> GetItemInBasket(string customerId, string itemId)
        {
            // Check if the basket exists
            if (!_basketRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket))
            {
                return BasketNotFound(customerId);
            }

            // Check if item exists in basket
            if (!_basketRepository.TryGetItemInBasket(itemId, customerId, out BasketItem item))
            {
                return ItemNotFound(customerId, itemId);
            }

            return item;
        }

        /// <summary>
        /// Add a product to a shopping basket
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> AddItemToBasket(string customerId, [FromBody] BasketItem item)
        {
            // Check if the basket cannot be found
            if (!_basketRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket))
            {
                return BasketNotFound(customerId);
            }

            // Check if item already exists in basket
            if (_basketRepository.TryGetItemInBasket(item.ProductId, customerId, out BasketItem existingItem))
            {
                return ItemAlreadyFound(customerId, item.ProductId);
            }

            _basketService.AddItemToBasket(shoppingBasket, item);

            return CreatedAtRoute("GetItemInBasket", new { customerId, itemId = item.ProductId }, item);
        }

        /// <summary>
        /// Updates the quantity of of a product in a shopping basket
        /// </summary>
        [HttpPatch("{itemId}")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<BasketItem> UpdateQuantity(string customerId, string itemId, [FromBody]JsonPatchDocument<BasketItem> patch)
        {
            // Try to get a basket to add the item to
            if (!_basketRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket))
            {
                return BasketNotFound(customerId);
            }

            // Check if item already exists in basket
            if (!_basketRepository.TryGetItemInBasket(itemId, customerId, out BasketItem item))
            {
                return ItemNotFound(customerId, itemId);
            }

            // Validate change and update via service
            if (!_basketService.TryUpdateItemInBasket(shoppingBasket, item, patch, out ICollection<ValidationResult> validationResults))
            {
                return BadRequest(validationResults);
            }

            return item;
        }

        /// <summary>
        /// Delete an item from a shopping basket
        /// </summary>
        [HttpDelete("{itemId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult RemoveItemFromBasket(string customerId, string itemId)
        {
            // Try to get a basket to remove the item from
            if (!_basketRepository.TryGetBasket(customerId, out ShoppingBasket shoppingBasket))
            {
                return BasketNotFound(customerId);
            }

            // Check if item already exists in basket
            if (!_basketRepository.TryGetItemInBasket(itemId, customerId, out BasketItem item))
            {
                return ItemNotFound(customerId, itemId);
            }

            _basketService.RemoveItemFromBasket(shoppingBasket, item);

            return NoContent();
        }

        #region Private methods
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
        #endregion
    }
}
