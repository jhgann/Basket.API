using System;
using System.Net.Http;
using Basket.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace EndToEndTests.Setup
{
    public class TestContext : IDisposable
    {
        private TestServer _server;
        public HttpClient Client { get; private set; }

        public TestContext()
        {
            SetUpClient();
        }

        private void SetUpClient()
        {
            // TODO: Currently disabling message bus during testing as it throws errors while trying to connect.
            //       The message bus only exists when running the app in docker.
            //       Will need to get it working or see about mocking it.
            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.Testing.json", optional: false) 
                .AddEnvironmentVariables()
                .Build();

            _server = new TestServer(new WebHostBuilder()
                .UseConfiguration(config)
                .UseStartup<Startup>());

            Client = _server.CreateClient();
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    _server?.Dispose();
                    Client?.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
