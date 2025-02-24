using System.Data;
using Futurist.Repository.Interface;
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
        
        services.AddScoped<IDbConnection>(s =>
        {
            var config = s.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        });
        services.AddScoped(s =>
        {
            var conn = s.GetRequiredService<IDbConnection>();
            conn.Open();
            return conn.BeginTransaction();
        });
    }
}