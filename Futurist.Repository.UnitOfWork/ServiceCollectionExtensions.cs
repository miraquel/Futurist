using Microsoft.Extensions.DependencyInjection;

namespace Futurist.Repository.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}