using System.Data;
using System.Security.Claims;
using Futurist.Infrastructure.SignalR.Hubs;
using Futurist.Repository.SqlServer;
using Futurist.Repository.UnitOfWork;
using Futurist.Service;
using Futurist.Web.Hangfire;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.Enrich.FromLogContext();
    
    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration.WriteTo.Console();
    }
    
    configuration
        .WriteTo
        .Logger(lc => lc
            .Filter.ByExcluding(Matching.FromSource("Futurist.Web.Hangfire.JobLoggerAttribute"))
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

    configuration
        .WriteTo
        .Logger(lc => lc
            .Filter.ByIncludingOnly(Matching.FromSource("Futurist.Web.Hangfire.JobLoggerAttribute"))
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
});

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .UseFilter(new JobLoggerAttribute())
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();

// Add SignalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie().AddOpenIdConnect(options =>
{
    var authConfig = builder.Configuration.GetSection("Auth");

    options.Authority = authConfig["Authority"];
    options.ClientId = authConfig["ClientId"];
    options.ClientSecret = authConfig["ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

builder.Services.AddRepositorySqlServer();
builder.Services.AddUnitOfWork();
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseHangfireDashboard();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();
app.MapHangfireDashboard();
app.MapHub<FuturistHub>("futuristHub");

app.Run();