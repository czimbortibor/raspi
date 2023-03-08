namespace RaspiInterface.Server.Modules;

public interface IModule
{
    IServiceCollection RegisterServices(IServiceCollection services);

    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder routes);
}

public static class ModuleExtensions
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = GetModules();
        foreach (var module in modules)
        {
            module.RegisterServices(services);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder routes)
    {
        var modules = GetModules();
        foreach (var module in modules)
        {
            module.MapEndpoints(routes);
        }

        return routes;
    }

    private static IEnumerable<IModule> GetModules()
    {
        return typeof(IModule)
            .Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}
