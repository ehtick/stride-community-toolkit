using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Stride.CommunityToolkit.Examples.MetadataGenerator;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Commands;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

// Configure Serilog early (before creating the host)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Stride Examples Metadata Generator");

    // Build the dependency injection container
    var builder = Host.CreateApplicationBuilder(args);

    // Configure Serilog from appsettings.json
    builder.Services.AddSerilog((services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services);
    });

    builder.Services.AddScoped<ManifestService>();
    builder.Services.AddScoped<ScanCommandHandler>();
    builder.Services.AddScoped<GenerateCommandHandler>();

    using var host = builder.Build();

    // Create and configure the CLI
    var cliConfiguration = new CommandLineConfiguration(host.Services);
    var rootCommand = cliConfiguration.CreateRootCommand();

    // Parse and invoke
    var parseResult = rootCommand.Parse(args);
    return parseResult.Invoke();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}
