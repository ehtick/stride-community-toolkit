using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Commands;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Services;
using System.CommandLine;

// Build the dependency injection container
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<ManifestService>();
builder.Services.AddScoped<ScanCommandHandler>();
builder.Services.AddScoped<GenerateCommandHandler>();

using var host = builder.Build();

// Define command-line structure
var pathArgument = new Argument<DirectoryInfo>("examples-root-path")
{
    Description = "The root path of the examples to scan.",
    DefaultValueFactory = _ => new DirectoryInfo(Path.Combine("..", "..", "..", "..", "..", "examples", "code-only"))
};

var scanCommand = new Command("scan", "Scans the examples and lists metadata.");
scanCommand.Arguments.Add(pathArgument);
scanCommand.SetAction(async (parseResult, cancellationToken) =>
{
    using var scope = host.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<ScanCommandHandler>();
    var path = parseResult.GetValue(pathArgument);
    return await handler.HandleAsync(path);
});

var generateCommand = new Command("generate", "Generates the metadata JSON manifest.");
generateCommand.Arguments.Add(pathArgument);
generateCommand.SetAction(async (parseResult, cancellationToken) =>
{
    using var scope = host.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<GenerateCommandHandler>();
    var path = parseResult.GetValue(pathArgument);
    return await handler.HandleAsync(path);
});

var rootCommand = new RootCommand("Stride examples metadata parser")
{
    scanCommand,
    generateCommand
};

var parseResult = rootCommand.Parse(args);

return parseResult.Invoke();