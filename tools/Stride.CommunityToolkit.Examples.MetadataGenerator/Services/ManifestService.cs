using Microsoft.Extensions.Logging;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

/// <summary>
/// Orchestrates the scanning, parsing, and generation of example metadata manifests.
/// </summary>
public class ManifestService(
    ILogger<ManifestService> logger,
    ExampleScanner exampleScanner,
    MetadataParser metadataParser,
    ManifestWriter manifestWriter)
{
    /// <summary>
    /// Scans the examples directory and collects metadata from all Program.cs files.
    /// </summary>
    /// <param name="examplesRootPath">The root directory containing example projects.</param>
    /// <returns>A collection of parsed example metadata.</returns>
    public async Task<List<ExampleMetadata>> ScanExamplesAsync(DirectoryInfo? examplesRootPath)
    {
        ArgumentNullException.ThrowIfNull(examplesRootPath);

        logger.LogInformation("Starting example scan in: {Path}", examplesRootPath.FullName);

        if (!examplesRootPath.Exists)
        {
            logger.LogError("Examples directory does not exist: {Path}", examplesRootPath.FullName);

            return [];
        }

        var examples = new List<ExampleMetadata>();
        var programFiles = exampleScanner.FindProgramFiles(examplesRootPath);

        foreach (var programFile in programFiles)
        {
            var projectName = exampleScanner.GetProjectName(programFile);
            logger.LogInformation("Processing example: {ProjectName}", projectName);

            try
            {
                var metadata = await metadataParser.ParseMetadataAsync(programFile, examplesRootPath.FullName);

                if (metadata is not null)
                {
                    examples.Add(metadata);
                    logger.LogInformation("".PadRight(20, ' ') + "✅ Parsed metadata, title: {ProjectName}", metadata.Title);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to process example: {ProjectName}", projectName);
            }
        }

        logger.LogInformation("Scan completed. Found {Count} examples with metadata", examples.Count);

        return examples;
    }

    /// <summary>
    /// Scans examples and generates a JSON manifest file.
    /// </summary>
    /// <param name="examplesRootPath">The root directory containing example projects.</param>
    /// <param name="outputPath">The path where the manifest JSON file should be written.</param>
    /// <returns>Exit code: 0 for success, 1 for failure.</returns>
    public async Task<int> ScanAndGenerateManifestAsync(DirectoryInfo? examplesRootPath, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(examplesRootPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        logger.LogInformation("Starting manifest generation for: {Path}", examplesRootPath.FullName);

        if (!examplesRootPath.Exists)
        {
            logger.LogError("Examples directory does not exist: {Path}", examplesRootPath.FullName);

            return 1;
        }

        var examples = await ScanExamplesAsync(examplesRootPath);

        if (examples.Count > 0)
        {
            await manifestWriter.WriteManifestAsync(examples, outputPath);

            logger.LogInformation("Manifest generation completed successfully");

            return 0;
        }

        logger.LogWarning("No examples with metadata found. No manifest written");

        return 0;
    }
}