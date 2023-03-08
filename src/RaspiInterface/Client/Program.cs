using RaspiInterface.Client;
using RaspiInterface.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly.Extensions.Http;
using Polly;
using Polly.Contrib.WaitAndRetry;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDeviceClient, DeviceClient>();
builder.Services
    .AddHttpClient<DeviceClient>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .SetHandlerLifetime(TimeSpan.FromMinutes(1))
    .AddPolicyHandler(GetRetryPolicy());

await builder.Build().RunAsync();



static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 3));
}