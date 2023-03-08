using RaspiInterface.Shared;

namespace RaspiInterface.Server.Modules.Devices.Endpoints;

public static class GetDevice
{
    public static IResult Handler(string deviceId)
    {
        var device = new DeviceDetails()
        {
            Id = Guid.NewGuid(),
            Name = "raspi",
            Location = new GeoLocation
            {
                Latitude = 52.304173,
                Longitude = 4.835277
            },
            ETag = Guid.NewGuid().ToString(),
            LastActivityTime = DateTime.Now,
            Status = DeviceStatus.Enabled
        };

        return Results.Ok(device);
    }
}
