using RaspiInterface.Shared;

namespace RaspiInterface.Server.Modules.Devices.Endpoints;

public static class GetDevices
{
    public static IResult Handler()
    {
        var devices = new List<Device>
            {
                new Device()
                {
                    Id = Guid.NewGuid(),
                    Name = "raspi",
                    Location = new GeoLocation
                    {
                        Latitude = 52.304173,
                        Longitude = 4.835277
                    }
                },

                new Device()
                {
                    Id = Guid.NewGuid(),
                    Name = "fake 1",
                    Location = new GeoLocation
                    {
                        Latitude = 52.292162,
                        Longitude = 4.830769
                    }
                },

                new Device()
                {
                    Id = Guid.NewGuid(),
                    Name = "fake 2",
                    Location = new GeoLocation
                    {
                        Latitude = 52.293726,
                        Longitude = 4.913641
                    }
                },

                new Device()
                {
                    Id = Guid.NewGuid(),
                    Name = "fake 3",
                    Location = new GeoLocation
                    {
                        Latitude = 52.305971,
                        Longitude = 4.953328
                    }
                }
            };


        return Results.Ok(devices);
    }
}
