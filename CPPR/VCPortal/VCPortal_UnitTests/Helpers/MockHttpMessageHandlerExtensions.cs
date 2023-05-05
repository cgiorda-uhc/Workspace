

namespace VCPortal_UnitTests.Helpers;
public static class MockHttpMessageHandlerExtensions
{
    public static IHttpClientFactory ToHttpClientFactory(this MockHttpMessageHandler mockHttpMessageHandler)
    {
        return new MockHttpClientFactory(mockHttpMessageHandler);
    }
}