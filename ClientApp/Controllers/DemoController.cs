using ClientApp.Config;
using ClientApp.IntegrationEvents.Events;
using ClientApp.Models;
using EventBusCore.Abstractions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;
        private readonly IOptions<ClientAppSettings> _settings;
        private readonly IEventBus _eventBus;

        public DemoController(IHttpClientFactory httpClientFactory, IOptions<ClientAppSettings> settings, IEventBus eventBus)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _client = _httpClientFactory.CreateClient("basket");
            _eventBus = eventBus;
        }

        [HttpGet]
        [Route("Basket/{customerId}")]
        public async Task<ActionResult<string>> GetBasket(string customerId)
        {
            var response = await _client.GetAsync($"/api/v1/shoppingbaskets/{customerId}");
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }
            return result;
        }

        [HttpPost]
        [Route("CreateBasket")]
        public async Task<ActionResult<string>> CreateBasket([FromBody] ShoppingBasket basket)
        {
            var response = await _client.PostAsJsonAsync($"/api/v1/shoppingbaskets", basket);
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            return Created(response.Headers.Location, result);
        }

        [HttpDelete]
        [Route("DeleteBasket/{customerId}")]
        public async Task<ActionResult> DeleteBasket(string customerId)
        {
            var response = await _client.DeleteAsync($"/api/v1/shoppingbaskets/{customerId}");
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }
            return NoContent();
        }

        [HttpPost]
        [Route("AddProductToBasket")]
        public async Task<ActionResult<string>> AddProductToBasket(string customerId, [FromBody] BasketItem item)
        {
            var response = await _client.PostAsJsonAsync($"/api/v1/shoppingbaskets/{customerId}/basketitems", item);
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }
            return Created(response.Headers.Location, result);
        }

        [HttpGet]
        [Route("Basket/{customerId}/BasketItems/{itemId}")]
        public async Task<ActionResult<string>> GetItemInBasket(string customerId, string itemId)
        {
            var response = await _client.GetAsync($"/api/v1/shoppingbaskets/{customerId}/basketitems/{itemId}");
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }
            return result;
        }

        [HttpPost]
        [Route("Basket/{customerId}/BasketItems/{itemId}/UpdateQuantity")]
        [ProducesResponseType(typeof(BasketItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BasketItem>> UpdateQuantity(string customerId, string itemId, [FromQuery] int newQuantity)
        {

            var patch = new JsonPatchDocument<BasketItem>().Replace(x => x.Quantity, newQuantity);
            var jsonString = JsonConvert.SerializeObject(patch);
            var response = await _client.PatchAsync($"/api/v1/shoppingbaskets/{customerId}/basketitems/{itemId}", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            var result = response.Content.ReadAsStringAsync().Result;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(result);
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(result);
            }

            var resultModel = JsonConvert.DeserializeObject<BasketItem>(result);
            return resultModel;
        }

        [HttpPost]
        [Route("{productId}/SimulatePriceChange")]
        public ActionResult SimulatePriceChange(string productId, [FromQuery] decimal newPrice)
        {
            var @event = new ProductPriceChangedIntegrationEvent(productId, newPrice);

            // Publish integration event to the event bus
            // (RabbitMQ or a service bus underneath)
            _eventBus.Publish(@event);
            return Ok();
        }
    }
}
