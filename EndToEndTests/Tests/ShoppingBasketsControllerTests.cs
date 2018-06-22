using Basket.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
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

        [TestMethod]
        public async Task ShoppingBasketsPutReturnsOkResponse()
        {
            var item = new ShoppingBasket
            {
                CustomerId = "1"
            };
            var jsonString = JsonConvert.SerializeObject(item);

            HttpResponseMessage response = await _context.Client.PutAsync(new Uri("/api/shoppingbaskets", UriKind.Relative), new StringContent(jsonString, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

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
