using System.Data;
using Futurist.Repository.Interface;
using Futurist.Repository.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Futurist.Repository.SqlServer;

public static class ServiceCollectionExtensions
{
    public static void AddRepositorySqlServer(this IServiceCollection services)
    {
        // TODO: Add repository services
        services.AddScoped<IRofoRepository, RofoRepository>();
        services.AddScoped<IBomStdRepository, BomStdRepository>();
        services.AddScoped<ICommonRepository, CommonRepository>();
        services.AddScoped<IMupRepository, MupRepository>();
        services.AddScoped<IFgCostRepository, FgCostRepository>();
        services.AddScoped<IJobMonitoringRepository, JobMonitoringRepository>();
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddScoped<IItemAdjustmentRepository, ItemAdjustmentRepository>();
        services.AddScoped<IFgCostVerRepository, FgCostVerRepository>();
        services.AddScoped<IItemForecastRepository, ItemForecastRepository>();
        
        services.AddScoped<IDbConnection>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        });
        services.AddScoped<Func<IDbTransaction>>(provider => 
        {
            var connection = provider.GetRequiredService<IDbConnection>();
            return () => connection.BeginTransaction();
        });
    }
}