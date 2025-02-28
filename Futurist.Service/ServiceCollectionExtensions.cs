using Futurist.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Futurist.Service;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        // TODO: Add services here
        services.AddScoped<MapperlyMapper>();
        services.AddScoped<IRofoService, RofoService>();
        services.AddScoped<IBomStdService, BomStdService>();
        services.AddScoped<IMupService, MupService>();
    }
}