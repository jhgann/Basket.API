using Basket.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TestContext = EndToEndTests.Setup.TestContext;

namespace EndToEndTests.Tests
{
    [TestClass]
    public class ShoppingBasketsControllerTests : IDisposable
    {
        private readonly TestContext _context;

        public ShoppingBasketsControllerTests()
        {
            _context = new TestContext();
        }

        #region POST TESTS
        [TestMethod]
        public async Task ShoppingBasketsPostReturnsCreated()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var response = await PostBasket(basket);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task ShoppingBasketsPostBadContentReturnsBadRequest()
        {
            var basket = new ShoppingBasket {};

            var response = await PostBasket(basket);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        #endregion

        #region GET TESTS
        [TestMethod]
        public async Task ShoppingBasketsGetByIdReturnsOk()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var postResponse = await PostBasket(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var response = await GetAsync(basket.CustomerId);
            var result = response.Content.ReadAsStringAsync().Result;
            var resultModel = JsonConvert.DeserializeObject<ShoppingBasket>(result);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(result, postResult);
            Assert.AreEqual(basket.CustomerId, resultModel.CustomerId);
        }

        [TestMethod]
        public async Task ShoppingBasketsGetByWrongIdReturnsNotFound()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var postResponse = await PostBasket(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var response = await GetAsync("2");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        #endregion

        #region DELETE TESTS
        [TestMethod]
        public async Task ShoppingBasketsDeleteByIdReturnsOk()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var postResponse = await PostBasket(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync(basket.CustomerId);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task ShoppingBasketsDeleteByIdThenGetReturnsNotFound()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var postResponse = await PostBasket(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync(basket.CustomerId);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var response2 = await GetAsync(basket.CustomerId);
            Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
        }

        [TestMethod]
        public async Task ShoppingBasketsDeleteByWrongIdReturnsNotFound()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var postResponse = await PostBasket(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync("2");
            var result = response.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task ShoppingBasketsDeleteTwiceReturnsNotFound()
        {
            var basket = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var postResponse = await PostBasket(basket);
            var postResult = postResponse.Content.ReadAsStringAsync().Result;

            var response = await DeleteAsync(basket.CustomerId);
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var response2 = await DeleteAsync(basket.CustomerId);
            Assert.AreEqual(HttpStatusCode.NotFound, response2.StatusCode);
        }
        #endregion

        private async Task<HttpResponseMessage> PostBasket(ShoppingBasket basket) => 
            await _context.Client.PostAsJsonAsync("/api/v1/shoppingbaskets", basket);

        private async Task<HttpResponseMessage> GetAsync(string customerId) => 
            await _context.Client.GetAsync(new Uri($"/api/v1/shoppingbaskets/{customerId}", UriKind.Relative));

        private async Task<HttpResponseMessage> DeleteAsync(string customerId) => 
            await _context.Client.DeleteAsync(new Uri($"/api/v1/shoppingbaskets/{customerId}", UriKind.Relative));

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
