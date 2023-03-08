using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RaspiInterface.Server.Tests;

[Trait("Category", "Integration")]
public abstract class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient client;

    public IntegrationTest(WebApplicationFactory<Program> fixture)
    {
        client = fixture.CreateClient();
    }
}
