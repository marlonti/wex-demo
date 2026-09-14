using System.Net;
using System.Text;

namespace api.Tests;

internal sealed class FakeHttpMessageHandler(string jsonResponse) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json"),
        };

        return Task.FromResult(response);
    }
}
