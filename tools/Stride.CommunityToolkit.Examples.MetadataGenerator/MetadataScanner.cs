using System.Text.RegularExpressions;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator;

/// <summary>
/// Scans Program.cs files in examples/code-only and extracts YAML metadata into JSON manifest.
/// </summary>
public partial class MetadataScanner(string examplesRoot, string outputPath)
{
    private readonly string _examplesRoot = examplesRoot;
    private readonly string _outputPath = outputPath;

    [GeneratedRegex(@"/\*\s*---example-metadata\s*(.*?)\s*---\s*\*/", RegexOptions.Singleline | RegexOptions.Compiled)]
    public static partial Regex YamlBlockRegex();
}