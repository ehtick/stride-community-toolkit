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

var rootCommand = CommandLineConfiguration.CreateRootCommand(host.Services);
var parseResult = rootCommand.Parse(args);

return parseResult.Invoke();