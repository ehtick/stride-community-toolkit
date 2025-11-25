using Microsoft.Extensions.DependencyInjection;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Services;
using System.CommandLine;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator;

/// <summary>
/// Builds and configures the command-line interface structure.
/// </summary>
/// <param name="serviceProvider">Service provider for resolving command handlers.</param>
public class CommandLineConfiguration(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Creates and configures the root command with all subcommands.
    /// </summary>
    /// <returns>Configured root command ready for parsing.</returns>
    public RootCommand CreateRootCommand()
    {
        var pathArgument = CreatePathArgument();
        var scanCommand = CreateScanCommand(pathArgument);
        var generateCommand = CreateGenerateCommand(pathArgument);

        return new RootCommand("Stride examples metadata parser")
        {
            scanCommand,
            generateCommand
        };
    }

    private static Argument<DirectoryInfo> CreatePathArgument()
    {
        return new Argument<DirectoryInfo>("examples-root-path")
        {
            Description = "The root path of the examples to scan.",
            DefaultValueFactory = _ => new DirectoryInfo(Path.Combine("..", "..", "examples", "code-only"))
        };
    }

    private Command CreateScanCommand(Argument<DirectoryInfo> pathArgument)
    {
        var scanCommand = new Command("scan", "Scans the examples and lists metadata.");
        scanCommand.Arguments.Add(pathArgument);
        scanCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ManifestService>();
            var path = parseResult.GetValue(pathArgument);

            await service.ScanExamplesAsync(path);

            return 0;
        });

        return scanCommand;
    }

    private Command CreateGenerateCommand(Argument<DirectoryInfo> pathArgument)
    {
        var generateCommand = new Command("generate", "Generates the metadata JSON manifest.");
        generateCommand.Arguments.Add(pathArgument);
        generateCommand.SetAction(async (parseResult, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ManifestService>();
            var path = parseResult.GetValue(pathArgument);

            return await service.GenerateManifestAsync(path);
        });

        return generateCommand;
    }
}