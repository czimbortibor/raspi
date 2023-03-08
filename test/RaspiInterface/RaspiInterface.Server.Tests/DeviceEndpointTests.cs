using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RaspiInterface.Shared;
using System.Net;
using Xunit;

namespace RaspiInterface.Server.Tests;

public class DeviceEndpointTests : IntegrationTest
{
    public DeviceEndpointTests(WebApplicationFactory<Program> fixture)
        : base(fixture) { }

    [Fact]
    public async Task Retrieves_the_devices()
    {
        var response = await client.GetAsync("/api/devices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var devices = JsonConvert.DeserializeObject<Device[]>(
          await response.Content.ReadAsStringAsync()
        );
        devices.Should().NotBeEmpty();
    }
}
