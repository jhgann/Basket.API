using Basket.Domain.Aggregates;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestContext = EndToEndTests.Setup.TestContext;

namespace EndToEndTests.Tests
{
    [TestClass]
    public class BasketItemsControllerTests : IDisposable
    {
        private readonly TestContext _context;

        public BasketItemsControllerTests()
        {
            _context = new TestContext();
        }

        #region POST TESTS
        [TestMethod]
        public async Task BasketItemsPostReturnsCreatedAndItem()
        {
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var response2 = await PostBasketItemAsync(basket, item);
            var result = response2.Content.ReadAsStringAsync().Result;
            var resultModel = JsonConvert.DeserializeObject<BasketItem>(result);

            response2.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.Created, response2.StatusCode);
            Assert.AreEqual(item.ProductId, resultModel.ProductId);
            Assert.AreEqual(item.ProductName, resultModel.ProductName);
            Assert.AreEqual(item.ProductPrice, resultModel.ProductPrice);
            Assert.AreEqual(item.Quantity, resultModel.Quantity);
        }

        [TestMethod]
        public async Task BasketItemsPostBadContentReturnsBadRequest()
        {
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(null,null,0,0);
            var response2 = await PostBasketItemAsync(basket, item);

            Assert.AreEqual(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [TestMethod]
        public async Task BasketItemsPostWithWrongBasketIdReturnsNotFound()
        {
            var badBasket = new ShoppingBasket("2");
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(badBasket, item);
            var result = postResponse2.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, postResponse2.StatusCode);
            Assert.AreEqual($"Basket for customer {badBasket.CustomerId} not found.", result);
        }

        [TestMethod]
        public async Task BasketItemsPostSameItemTwiceReturnsBadRequest()
        {
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var postResponse3 = await PostBasketItemAsync(basket, item);
            var result = postResponse3.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse3.StatusCode);
            Assert.AreEqual($"Product {item.ProductId} already in basket for customer {basket.CustomerId}, and cannot be added again.", result);
        }
        #endregion

        #region PATCH TESTS
        [TestMethod]
        public async Task BasketItemsChangeQuantityReturnsOkAndItem()
        {
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var response2 = await PostBasketItemAsync(basket, item);

            var newQuantity = 5;
            var patch = new JsonPatchDocument<BasketItem>().Replace(x => x.Quantity, newQuantity);
            
            var response3 = await PutItemQuantityAsync(basket, item, newQuantity);
            var result = response3.Content.ReadAsStringAsync().Result;
            var resultModel = JsonConvert.DeserializeObject<BasketItem>(result);

            response3.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response3.StatusCode);
            Assert.AreEqual(item.ProductId, resultModel.ProductId);
            Assert.AreEqual(item.ProductName, resultModel.ProductName);
            Assert.AreEqual(item.ProductPrice, resultModel.ProductPrice);
            Assert.AreEqual(newQuantity, resultModel.Quantity);
        }

        [TestMethod]
        public async Task BasketItemsChangeQuantityOutOfBoundsReturnsBadRequest()
        {
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var response2 = await PostBasketItemAsync(basket, item);

            var newQuantity = 0;
            var patch = new JsonPatchDocument<BasketItem>().Replace(x => x.Quantity, newQuantity);
            var response3 = await PutItemQuantityAsync(basket, item, newQuantity);
            var result = response3.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, response3.StatusCode);
            Assert.AreEqual("[{\"memberNames\":[\"Quantity\"],\"errorMessage\":\"Quantity must be between 1 and 999.\"}]",result);
        }

        [TestMethod]
        public async Task BasketItemsChangeQuantityWithWrongBasketReturnsNotFound()
        {
            var wrongBasket = new ShoppingBasket("2");
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var response2 = await PostBasketItemAsync(basket, item);

            var newQuantity = 5;
            var patch = new JsonPatchDocument<BasketItem>().Replace(x => x.Quantity, newQuantity);

            var response3 = await PutItemQuantityAsync(wrongBasket, item, newQuantity);
            var result = response3.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response3.StatusCode);
            Assert.AreEqual($"Basket for customer {wrongBasket.CustomerId} not found.", result);
        }

        [TestMethod]
        public async Task BasketItemsChangeQuantityWithWrongBasketItemReturnsNotFound()
        {
            var basket = new ShoppingBasket("1");
            var response = await PostBasketAsync(basket);
            response.EnsureSuccessStatusCode();

            var wrongItem = new BasketItem(Guid.NewGuid().ToString(), "Product Name2", 2.99M, 2);
            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var response2 = await PostBasketItemAsync(basket, item);

            var newQuantity = 5;
            var patch = new JsonPatchDocument<BasketItem>().Replace(x => x.Quantity, newQuantity);

            var response3 = await PutItemQuantityAsync(basket, wrongItem, newQuantity);
            var result = response3.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response3.StatusCode);
            Assert.AreEqual($"Product {wrongItem.ProductId} could not be found in basket for customer {basket.CustomerId}.", result);
        }
        #endregion

        #region GET TESTS
        [TestMethod]
        public async Task BasketItemsGetAllReturnsOkAndItems()
        {
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var item2 = new BasketItem(Guid.NewGuid().ToString(), "Product Name2", 2.99M, 2);
            var postResponse3 = await PostBasketItemAsync(basket, item2);

            var response = await GetAsync(basket.CustomerId);
            var result = response.Content.ReadAsStringAsync().Result;
            var resultModel = JsonConvert.DeserializeObject<List<BasketItem>>(result);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(resultModel.Exists(x => x.ProductId == item.ProductId));
            Assert.IsTrue(resultModel.Exists(x => x.ProductId == item2.ProductId));
        }

        [TestMethod]
        public async Task BasketItemsGetAllByWrongIdReturnsNotFound()
        {
            var badCustomerId = "2";
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var postResult2 = postResponse2.Content.ReadAsStringAsync().Result;

            var response = await GetAsync(badCustomerId);
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual($"Basket for customer {badCustomerId} not found.", result);
        }

        [TestMethod]
        public async Task BasketItemsGetByIdReturnsOk()
        {
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var postResult2 = postResponse2.Content.ReadAsStringAsync().Result;

            var response = await GetAsync(basket.CustomerId, item.ProductId);
            var result = response.Content.ReadAsStringAsync().Result;
            var resultModel = JsonConvert.DeserializeObject<BasketItem>(result);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(postResult2, result);
            Assert.AreEqual(item.ProductId, resultModel.ProductId);
            Assert.AreEqual(item.ProductName, resultModel.ProductName);
            Assert.AreEqual(item.ProductPrice, resultModel.ProductPrice);
            Assert.AreEqual(item.Quantity, resultModel.Quantity);
        }

        [TestMethod]
        public async Task BasketItemsGetByWrongItemIdReturnsNotFound()
        {
            var badItemId = "wrongItemId";
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);

            var response = await GetAsync(basket.CustomerId, badItemId);
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual($"Product {badItemId} could not be found in basket for customer {basket.CustomerId}.", result);
        }

        [TestMethod]
        public async Task BasketItemsGetByWrongCustomerIdReturnsNotFound()
        {
            var badCustomerId = "2";
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);

            var response = await GetAsync(badCustomerId, item.ProductId);
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual($"Basket for customer {badCustomerId} not found.", result);
        }
        #endregion

        #region DELETE TESTS
        [TestMethod]
        public async Task BasketItemsDeleteByIdReturnsOk()
        {
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var postResult2 = postResponse2.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync(basket.CustomerId, item.ProductId);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task BasketItemsDeleteByIdThenGetReturnsNotFound()
        {
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var postResult2 = postResponse2.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync(basket.CustomerId, item.ProductId);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var response2 = await GetAsync(basket.CustomerId, item.ProductId);
            Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
        }

        [TestMethod]
        public async Task BasketItemsDeleteByWrongIdReturnsNotFound()
        {
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);
            var postResult2 = postResponse2.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync(basket.CustomerId, "wrongItemId");
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task BasketItemsDeleteTwiceReturnsNotFound()
        {
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);

            var response = await DeleteAsync(basket.CustomerId, item.ProductId);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var response2 = await DeleteAsync(basket.CustomerId, item.ProductId);
            var result = response2.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
            Assert.AreEqual($"Product {item.ProductId} could not be found in basket for customer {basket.CustomerId}.", result);
        }

        [TestMethod]
        public async Task BasketItemsDeleteFromWrongBasketReturnsNotFound()
        {
            var badCustomerId = "2";
            var basket = new ShoppingBasket("1");
            var postResponse = await PostBasketAsync(basket);

            var item = new BasketItem(Guid.NewGuid().ToString(), "Product Name", 1.99M, 1);
            var postResponse2 = await PostBasketItemAsync(basket, item);

            var response = await DeleteAsync(badCustomerId, item.ProductId);
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual($"Basket for customer {badCustomerId} not found.", result);
        }
        #endregion

        private async Task<HttpResponseMessage> PostBasketAsync(ShoppingBasket basket) => 
            await _context.Client.PostAsJsonAsync("/api/v1/shoppingbaskets", basket);

        private async Task<HttpResponseMessage> PostBasketItemAsync(ShoppingBasket basket, BasketItem item) => 
            await _context.Client.PostAsJsonAsync($"/api/v1/shoppingbaskets/{basket.CustomerId}/basketitems", item);

        private async Task<HttpResponseMessage> PutItemQuantityAsync(ShoppingBasket basket, BasketItem item, int quantity) => 
            await _context.Client.PutAsJsonAsync(new Uri($"/api/v1/shoppingbaskets/{basket.CustomerId}/basketitems/{item.ProductId}", UriKind.Relative), quantity);

        private async Task<HttpResponseMessage> GetAsync(string customerId) =>
            await _context.Client.GetAsync(new Uri($"/api/v1/shoppingbaskets/{customerId}/basketitems", UriKind.Relative));

        private async Task<HttpResponseMessage> GetAsync(string customerId, string itemId) => 
            await _context.Client.GetAsync(new Uri($"/api/v1/shoppingbaskets/{customerId}/basketitems/{itemId}", UriKind.Relative));

        private async Task<HttpResponseMessage> DeleteAsync(string customerId, string itemId) => 
            await _context.Client.DeleteAsync(new Uri($"/api/v1/shoppingbaskets/{customerId}/basketitems/{itemId}", UriKind.Relative));

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }

        [TestCleanup]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
