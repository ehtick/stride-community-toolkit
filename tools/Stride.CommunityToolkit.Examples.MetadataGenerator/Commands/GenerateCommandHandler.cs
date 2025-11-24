using Microsoft.Extensions.Logging;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Commands;

/// <summary>
/// Handler for the generate command that creates the metadata JSON manifest.
/// </summary>
/// <param name="manifestService">Service for manifest generation.</param>
/// <param name="logger">Logger instance for output.</param>
public class GenerateCommandHandler(ManifestService manifestService, ILogger<GenerateCommandHandler> logger)
{
    public Task<int> HandleAsync(DirectoryInfo examplesRootPath)
    {
        //Debugger.Launch();

        logger.LogInformation("Executing generate command...");
        logger.LogInformation("Examples root path: {Path}", examplesRootPath.FullName);

        manifestService.GenerateManifest();

        return Task.FromResult(0);
    }
}