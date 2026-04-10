using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SnapMyBook.Web.Tests.Endpoints;

public class RootRedirectTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RootRedirectTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Root_ShouldRedirectAnonymousUsersToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/auth/login", response.Headers.Location?.OriginalString);
    }
}
