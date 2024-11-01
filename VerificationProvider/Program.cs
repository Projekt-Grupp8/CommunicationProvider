using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DatabaseContexts>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("Star_db")));
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IVerificationCleanerService, VerificationCleanerService>();
        services.AddScoped<IValidateVerificationCodeService, ValidateVerificationCodeService>();
        services.AddScoped<IWelcomeService, WelcomeService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContexts>();
        var migration = context.Database.GetPendingMigrations();
        if(migration != null && migration.Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"ERROR : Program.cs :: {ex.Message}");
    }
}

host.Run();
