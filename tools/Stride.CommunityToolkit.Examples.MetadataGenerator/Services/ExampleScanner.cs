using Microsoft.Extensions.Logging;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

/// <summary>
/// Scans the file system for example projects containing metadata.
/// </summary>
public class ExampleScanner(ILogger<ExampleScanner> logger)
{
    private const string ProgramFileName = "Program.cs";

    /// <summary>
    /// Finds all Program.cs files in the specified directory and its subdirectories.
    /// </summary>
    /// <param name="examplesRootPath">The root directory to scan.</param>
    /// <returns>A collection of file paths to Program.cs files.</returns>
    public IEnumerable<string> FindProgramFiles(DirectoryInfo examplesRootPath)
    {
        ArgumentNullException.ThrowIfNull(examplesRootPath);

        logger.LogInformation("Scanning for {FileName} files in: {Path}", ProgramFileName, examplesRootPath.FullName);

        if (!examplesRootPath.Exists)
        {
            logger.LogError("Examples directory does not exist: {Path}", examplesRootPath.FullName);
            return [];
        }

        var programFiles = Directory.GetFiles(examplesRootPath.FullName, ProgramFileName, SearchOption.AllDirectories);

        logger.LogInformation("Found {Count} {FileName} files", programFiles.Length, ProgramFileName);

        return programFiles;
    }

    /// <summary>
    /// Gets the project name from a Program.cs file path.
    /// </summary>
    /// <param name="programFilePath">The full path to the Program.cs file.</param>
    /// <returns>The project directory name.</returns>
    public string GetProjectName(string programFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(programFilePath);

        var projectDirectory = Path.GetDirectoryName(programFilePath);

        return Path.GetFileName(projectDirectory) ?? string.Empty;
    }
}