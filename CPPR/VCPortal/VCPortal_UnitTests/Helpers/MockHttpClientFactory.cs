

namespace VCPortal_UnitTests.Helpers;
public class MockHttpClientFactory : IHttpClientFactory
    {
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;

        public MockHttpClientFactory(MockHttpMessageHandler mockHttpMessageHandler)
        {
            _mockHttpMessageHandler = mockHttpMessageHandler ?? throw new ArgumentNullException(nameof(mockHttpMessageHandler));
        }

        public System.Net.Http.HttpClient CreateClient(string name)
        {
            return _mockHttpMessageHandler.ToHttpClient();
        }
    }

