using Microsoft.Extensions.Logging;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Commands;

/// <summary>
/// Handler for the scan command that extracts and displays example metadata.
/// </summary>
/// <param name="logger">Logger instance for output.</param>
public class ScanCommandHandler(ILogger<ScanCommandHandler> logger)
{
    public async Task<int> HandleAsync(DirectoryInfo examplesRootPath)
    {
        logger.LogInformation("Executing scan command...");
        logger.LogInformation("Examples root path: {Path}, exists: {Exists}",
            examplesRootPath.FullName, examplesRootPath.Exists);

        if (!examplesRootPath.Exists)
        {
            logger.LogError("Directory does not exist: {Path}", examplesRootPath.FullName);
            return 1;
        }

        var scanner = new MetadataScanner(examplesRootPath.FullName, "examples-metadata.json");
        await scanner.ScanExamplesAsync();

        return 0;
    }
}