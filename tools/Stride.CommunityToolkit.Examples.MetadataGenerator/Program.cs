using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stride.CommunityToolkit.Examples.MetadataGenerator;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Commands;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

// Build the dependency injection container
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ManifestService>();
builder.Services.AddScoped<ScanCommandHandler>();
builder.Services.AddScoped<GenerateCommandHandler>();

using var host = builder.Build();

// Create and configure the CLI
var cliConfiguration = new CommandLineConfiguration(host.Services);
var rootCommand = cliConfiguration.CreateRootCommand();

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();