using RaspiInterface.Server.Modules.Devices.Endpoints;
using RaspiInterface.Shared;

namespace RaspiInterface.Server.Modules.Devices;

public class DeviceModule : IModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder routes)
    {
        var endpointsGroup = 
            routes
            .MapGroup("/api/devices")
            .WithTags(nameof(Device))
            .WithOpenApi();

        endpointsGroup
            .MapGet("/", GetDevices.Handler)
            .Produces<List<Device>>(200);

        return endpointsGroup;
    }
}
