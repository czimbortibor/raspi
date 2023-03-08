using RaspiInterface.Shared;
using System.Net.Http.Json;

namespace RaspiInterface.Client.Services;

public class DeviceClient : IDeviceClient
{
    private readonly HttpClient httpClient;

    public DeviceClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<Device>> GetDevices() 
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<IEnumerable<Device>>("api/devices");
            return response ?? Enumerable.Empty<Device>();
        }
        catch (Exception)
        {
            // just propagate for now
            throw;
        }
    }
}
