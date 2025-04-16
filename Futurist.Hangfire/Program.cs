using System.Data;
using Futurist.Hangfire;
using Futurist.Repository.SqlServer;
using Futurist.Repository.UnitOfWork;
using Futurist.Service;
using Hangfire;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRepositorySqlServer();
builder.Services.AddUnitOfWork();
builder.Services.AddServices();

// Add Serilog
var serilogConfiguration = new LoggerConfiguration();

serilogConfiguration.ReadFrom.Configuration(builder.Configuration);
serilogConfiguration.Enrich.FromLogContext();
if (builder.Environment.IsDevelopment())
{
    serilogConfiguration.WriteTo.Console();
}

serilogConfiguration.WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(Matching.FromSource("Futurist.Hangfire.JobLoggerAttribute"))
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("SerilogConnection"), 
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
        },
        columnOptions: new ColumnOptions
        {
            AdditionalColumns =
            [
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "SourceContext" }
            ]
        }
    )
);

serilogConfiguration.WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(Matching.FromSource("Futurist.Hangfire.JobLoggerAttribute"))
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("SerilogConnection"), 
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "JobLogs",
            AutoCreateSqlTable = true,
        },
        columnOptions: new ColumnOptions
        {
            AdditionalColumns =
            [
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "SourceContext" },
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "Status" },
                new SqlColumn { DataType = SqlDbType.VarChar, ColumnName = "JobId" }
            ]
        }
    )
);

var serilog = serilogConfiguration.CreateLogger();

Log.Logger = serilog;

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(serilog));

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .UseFilter(new JobLoggerAttribute())
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

builder.Services.AddHttpClient<KeycloakTokenService>();

var host = builder.Build();
host.Run();